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

    public CucumberSpawner cucumberSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        } while (!tilemap.HasTile(randomCell) || occupiedCells.Contains(randomCell));

        // Cell is occupied
        occupiedCells.Add(randomCell);

        Vector3 spawnPos = tilemap.GetCellCenterWorld(randomCell);

        // Spawn object
        GameObject obj = Instantiate(cat, spawnPos, Quaternion.identity);
        cucumberSpawner.spawnedObjects[randomCell] = obj;

        // Layering order
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = -(int)(spawnPos.y * 100);
    }
}
