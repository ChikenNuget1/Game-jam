using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveManager : MonoBehaviour
{
    public Spawner spawner;
    public Tilemap tilemap;

    public int waveNumber;

    bool waveInProgress = true;

    // Update is called once per frame
    void Update()
    {
        if (waveInProgress && isWaveComplete())
        {
            waveInProgress = false;
            nextWave();
        }
    }

    bool isWaveComplete()
    {
        var objects = spawner.spawnedObjects;

        if (objects.Count == 0)
        {
            return true;
        }
        foreach (var obj in objects)
        {
            Vector3Int cell = obj.Key;

            if (canMove(cell))
            {
                return false;
            }
        }
        return true;
    }

    bool canMove(Vector3Int cell)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        };

        foreach (var dir in directions)
        {
            Vector3Int cucumberCell = cell - dir; // where cucumber would go
            Vector3Int targetCell = cell + dir;   // where cat would be pushed

            if (tilemap.HasTile(cucumberCell) &&     // can place cucumber
                tilemap.HasTile(targetCell) &&       // destination exists
                !spawner.spawnedObjects.ContainsKey(targetCell)) // destination free
            {
                return true;
            }
        }

        return false;
    }

    void nextWave()
    {
        waveNumber++;
        clearAllCats();
        spawnNewWave();

        waveInProgress = true;
    }

    void clearAllCats()
    {
        foreach (var obj in spawner.spawnedObjects.Values)
        {
            Destroy(obj);
        }
    }

    void spawnNewWave()
    {
        spawner.spawn();
    }
}
