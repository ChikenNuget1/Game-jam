using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
    // public Dictionary<Vector3Int, GameObject> spawnedObjects = new Dictionary<Vector3Int, GameObject>();    // Might need to change this so that we track spawnedObjects through the cat spawner.

    public Spawner spawner;

    Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0), // x
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0), // y
        new Vector3Int(0, -1, 0),
        /* new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 1, 0),       If we want diagonal of the cucumbers to move
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, -1, 0), */
    };

    public Tilemap goalTileMap;
    public int scoreToAdd = 1;

    public ScoreManager scoreManager;

    void Update()
    {
        // MAIN CLICK LOGIC
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

                // Add delay before moving cats
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
            if (spawner.spawnedObjects.ContainsKey(neighborCell))
            {
                GameObject obj = spawner.spawnedObjects[neighborCell];
                Vector3Int targetCell = neighborCell + dir;

                // If cat exists on adjacent tile && a ground tile exists || a goal tile exists
                if (!spawner.spawnedObjects.ContainsKey(targetCell) && (tilemap.HasTile(targetCell) || goalTileMap.HasTile(targetCell)))
                {
                    spawner.spawnedObjects.Remove(neighborCell);
                    spawner.spawnedObjects[targetCell] = obj;

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
            spawner.spawnedObjects.Remove(targetCell);
            Destroy(obj);
            scoreManager.addScore(scoreToAdd);
        }
    }

    IEnumerator DelayAction(float delayTime, Vector3Int cellPos)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        foreach (var dir in directions)
        {
            Vector3Int neighbor = cellPos + dir;
            
            if (spawner.spawnedObjects.ContainsKey(neighbor))
            {
                PushChain(neighbor, dir);
            }
        }
        // PushNearby(cellPos);
    }

    /// <summary>
    /// Function that returns all cats that are next to other cats.
    /// </summary>
    /// <param name="startcell"> Cell where cucumber is placed </param>
    /// <returns> A hashset containing cats that are connected to each other </returns>
    HashSet<Vector3Int> GetConnectedCats(Vector3Int startcell)
    {
        // Cats to visit
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        
        // Cats that have been visited
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(startcell);
        visited.Add(startcell);

        Vector3Int[] xDirections = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0), // x
            new Vector3Int(-1, 0, 0),
            // new Vector3Int(0, 1, 0), // y
            // new Vector3Int(0, -1, 0),
            /* new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 1, 0),       If we want diagonal of the cucumbers to move
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, -1, 0), */
        };

        // While queue exists
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            // For each cat within 1 block of the cucumber
            // Check for other cats within 1 block of those cats
            // Iteratively check until no cats nearby
            foreach (var dir in xDirections)
            {
                var neighbor = current + dir;

                // If cats are next to each other
                if (spawner.spawnedObjects.ContainsKey(neighbor) && !visited.Contains(neighbor))
                {
                    // Add them to the visited list
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        return visited;
    }

    /// <summary>
    /// Pushes all cats that are next to each other OR facing each other.
    /// </summary>
    /// <param name="startcell"> Cell where cucumber is placed </param>
    /// <param name="dir"> Direction </param>
    void PushChain(Vector3Int startcell, Vector3Int dir)
    {
        var connected = GetConnectedCats(startcell);

        List<Vector3Int> sorted = new List<Vector3Int>(connected);
        sorted.Sort((a, b) => Dot(b, dir).CompareTo(Dot(a, dir)));

        HashSet<Vector3Int> occupied = new HashSet<Vector3Int>(spawner.spawnedObjects.Keys);

        List<Vector3Int> toRemove = new List<Vector3Int>();
        List<(Vector3Int oldCell, GameObject obj, Vector3 newPos, Vector3Int newCell)> moves = new List<(Vector3Int, GameObject, Vector3, Vector3Int)>();

        // Remove cells that are moving
        foreach (var cell in connected)
        {
            occupied.Remove(cell);
        }

        foreach (var cell in sorted)
        {
            Vector3Int target = cell + dir;

            if (goalTileMap.HasTile(target))
            {
                toRemove.Add(cell);
                continue;
            }

            if (!tilemap.HasTile(target) || occupied.Contains(target))
            {
                continue;
            }

            GameObject obj = spawner.spawnedObjects[cell];
            Vector3 newPos = tilemap.GetCellCenterWorld(target);

            moves.Add((cell, obj, newPos, target));
        }

        foreach (var move in moves)
        {
            spawner.spawnedObjects.Remove(move.oldCell);
        }

        foreach (var move in moves)
        {
            move.obj.transform.position = move.newPos;
            spawner.spawnedObjects[move.newCell] = move.obj;
        }

        HandleCombo(toRemove);
    }

    void HandleCombo(List<Vector3Int> removedCells)
    {
        int comboCount = removedCells.Count;

        if (comboCount == 0) return;

        // Exponential score modifer
        int totalScore = scoreToAdd * comboCount * comboCount;

        foreach (var cell in removedCells)
        {
            GameObject obj = spawner.spawnedObjects[cell];
            Destroy(obj);
            spawner.spawnedObjects.Remove(cell);
        }

        scoreManager.addScore(totalScore);

        Debug.Log("Combo x" + comboCount + " Score: " + totalScore);
    }

    int Dot(Vector3Int lhs, Vector3Int rhs)
    {
        return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
    }
}