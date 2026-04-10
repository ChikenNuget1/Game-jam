using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CucumberSpawner : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject cucumber;

    public float actionDelay = 5f;

    // Example: At vector [0, 0, 0], there exists a Cat.
    // [0, 0, 0] == GameObject Cat.
    public Dictionary<Vector3Int, GameObject> spawnedObjects = new Dictionary<Vector3Int, GameObject>();

    Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        /* new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 1, 0),       If we want diagonal of the cucumbers to move
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, -1, 0), */
    };

    public Tilemap goalTileMap;
    public int score = 0;
    public int scoreToAdd;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mouseScreen = Mouse.current.position.ReadValue();
            mouseScreen.z = -Camera.main.transform.position.z; // correct depth

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

            Vector3Int cellPos = tilemap.WorldToCell(mouseWorld);

            // Cucumber only spawns on tiles that exist and another cucumber doesn't already exist
            GameObject target = GameObject.FindWithTag("Cucumber");
            if (tilemap.HasTile(cellPos) && target == null)
            {
                Vector3 spawnPos = tilemap.GetCellCenterWorld(cellPos);

                GameObject obj = Instantiate(cucumber, spawnPos, Quaternion.identity);

                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

                StartCoroutine(DelayAction(actionDelay, cellPos));
                if (sr != null)
                    sr.sortingOrder = -(int)(spawnPos.y * 100);
            }
        }
    }

    /// <summary>
    ///  Function for pushing nearby cats one tile away.
    ///  
    /// This function pretty much checks all adjacent tiles from the cucumber and if there is a cat, then moves it one tile away
    /// </summary>
    /// <param name="centerCell"> The cell where the cucumber is located </param>
    void PushNearby(Vector3Int centerCell)
    {
        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighborCell = centerCell + dir;
            // Check adjacent cells
            if (spawnedObjects.ContainsKey(neighborCell))
            {
                GameObject obj = spawnedObjects[neighborCell];
                Vector3Int targetCell = neighborCell + dir;

                // If cat exists on adjacent tile && a ground tile exists || a goal tile exists
                if (!spawnedObjects.ContainsKey(targetCell) && (tilemap.HasTile(targetCell) || goalTileMap.HasTile(targetCell)))
                {
                    spawnedObjects.Remove(neighborCell);
                    spawnedObjects[targetCell] = obj;

                    Vector3 newPos = tilemap.GetCellCenterWorld(targetCell);
                    // Move one tile away
                    obj.transform.position = newPos;

                    // Check if a cat is at goal
                    checkGoal(targetCell, obj);

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                        sr.sortingOrder = -(int)(newPos.y * 100);
                }
            }
        }
    }

    void checkGoal(Vector3Int targetCell, GameObject obj)
    {
        // Check if the goal tile has a cat on it
        if (goalTileMap.HasTile(targetCell))
        {
            spawnedObjects.Remove(targetCell);
            Destroy(obj);
            score += scoreToAdd;

            Debug.Log("Score: " + score);
        }
    }

    IEnumerator DelayAction(float delayTime, Vector3Int cellPos)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        PushNearby(cellPos);
    }
}

// TODO: maybe chain-cat moving? Moving a cat can other cats?