using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    public GameObject[] lootItems = new GameObject[10];
    public GameObject player;
    public Generation mapgen;
    public List<GameObject> lootList;
    private GameObject[] lootObjects;
    int randX;
    int randY;
    // Start is called before the first frame update
    public void Start()
    {
        Spawning();
    }

    public void resetLoot()
    {
        lootList.Clear();
        lootObjects = GameObject.FindGameObjectsWithTag("Loot");
        foreach (GameObject item in lootObjects)
        {
            Destroy(item);
        }
    }

    void Spawning()
    {
        for (int i = 0; i < 30; i++)
        {
            lootList = new List<GameObject>();
            while (true)
            {
                randX = UnityEngine.Random.Range(-74, 75);
                randY = UnityEngine.Random.Range(-74, 75);
                if (mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX, randY, 0)) == mapgen.groundTile &&
                    mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX - 1, randY, 0)) == mapgen.groundTile &&
                    mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX + 1, randY, 0)) == mapgen.groundTile &&
                    mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX, randY - 1, 0)) == mapgen.groundTile &&
                    mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX, randY + 1, 0)) == mapgen.groundTile &&
                    Vector2.Distance(player.transform.position, new Vector2(randX, randY)) > 10 &&
                    !lootList.Any(e => e.transform.position == new Vector3(randX, randY, 0)))
                { break; }
            }
            int rand = Random.Range(0, lootItems.Count());
            Instantiate(lootItems[rand], new Vector2(randX+0.5f, randY + 0.5f), Quaternion.identity);
            lootList.Add(lootItems[rand]);
        }
    }
}
