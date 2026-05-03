using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour
{
    public GameObject cat;
    public Tilemap tilemap;

    public int spawnCount = 5;

    // Store occupied cells
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();

    // public CucumberSpawner cucumberSpawner;
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

    public Sprite[] catSprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnedObjects.Clear();
        spawn();
    }

    public void spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnRandomTile();
        }
        occupiedCells.Clear();
    }

    void SpawnRandomTile()
    {
        Vector3Int randomCell;
        
        // While cell doesn't exist or already contains a cat
        do
        {
            randomCell = new Vector3Int(
                Random.Range(tilemap.cellBounds.xMin, tilemap.cellBounds.xMax),
                Random.Range(tilemap.cellBounds.yMin, tilemap.cellBounds.yMax),
                0
            );
        } while (!tilemap.HasTile(randomCell) || occupiedCells.Contains(randomCell) || !isSafeTile(randomCell));

        // Cell is occupied
        occupiedCells.Add(randomCell);

        Vector3 spawnPos = tilemap.GetCellCenterWorld(randomCell);

        // Spawn object
        GameObject obj = Instantiate(cat, spawnPos, Quaternion.identity);
        obj.transform.localScale = new Vector3(3f, 3f, 3f);

        //cucumberSpawner.spawnedObjects[randomCell] = obj;
        spawnedObjects[randomCell] = obj;

        // Layering order
        SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            int index = Random.Range(0, catSprites.Length);
            sr.sprite = catSprites[index];
            sr.sortingOrder = -(int)(spawnPos.y);
        }
            
    }

    bool isSafeTile(Vector3Int cell)
    {
        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighbor = cell + dir;

            // If tile does not have neighbors then return false
            if (!tilemap.HasTile(neighbor))
            {
                return false;
            }
        }
        return true;
    }
}
