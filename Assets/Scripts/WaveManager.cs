using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveManager : MonoBehaviour
{
    public Spawner spawner;
    public Tilemap tilemap;

    public int waveNumber;

    bool waveInProgress = true;

    public TimerManager timerManager;
    public float timeBonus = 5f;

    // Check if wave is in progress or the wave is complete.
    void Update()
    {
        if (waveInProgress && isWaveComplete())
        {
            waveInProgress = false;
            nextWave();
        }
    }

    // Check if wave is cocmplete
    bool isWaveComplete()
    {
        var objects = spawner.spawnedObjects;

        if (objects.Count == 0)
        {
            return true;
        }
        // For each cat, check if they are still able to be moved
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

    // Check if cats can move
    bool canMove(Vector3Int cell)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        };

        // For each direction vertex, check if a cat can move / if a cucumber can be placed.
        foreach (var dir in directions)
        {
            Vector3Int cucumberCell = cell - dir; // Check cucumber placement
            Vector3Int targetCell = cell + dir;   // Check cat movement

            if (tilemap.HasTile(cucumberCell) &&     // can place cucumber
                tilemap.HasTile(targetCell) &&       // destination exists
                !spawner.spawnedObjects.ContainsKey(targetCell)) // destination free
            {
                return true;
            }
        }

        return false;
    }

    // Initialise new wave
    void nextWave()
    {
        waveNumber++;
        clearAllCats();
        spawnNewWave();
        timerManager.addTime(waveNumber);

        waveInProgress = true;
    }

    // Clear all cats off the map
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
