using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CucumberSpawner : MonoBehaviour
{

    public Tilemap tilemap;
    public GameObject cucumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mouseScreen = Mouse.current.position.ReadValue();
            mouseScreen.z = -Camera.main.transform.position.z; // correct depth

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

            Vector3Int cellPos = tilemap.WorldToCell(mouseWorld);

            // Cucumber only spawns on tiles that exist
            if (tilemap.HasTile(cellPos))
            {
                Vector3 spawnPos = tilemap.GetCellCenterWorld(cellPos);

                GameObject obj = Instantiate(cucumber, spawnPos, Quaternion.identity);

                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sortingOrder = -(int)(spawnPos.y * 100);
            }
        }
    }
}
