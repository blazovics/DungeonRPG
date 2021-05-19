using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public int level;
    public GameObject enemy;
    public GameObject player;
    public Generation mapgen;
    public List<GameObject> enemies;
    private GameObject[] enemyObjects;

    int randX;
    int randY;

    public void Start()
    {
        level = 2;
        Spawning();
    }

    public void ResetEnemy()
    {
        enemies.Clear();
        enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject item in enemyObjects)
        {
            Destroy(item);
        }
    }
    void Spawning()
    {
        for(int i = 0; i < level*40; i++)
        {
            enemies = new List<GameObject>();
            while (true)
            {
                randX = UnityEngine.Random.Range(-74, 75);
                randY = UnityEngine.Random.Range(-74, 75);
                if (mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX,randY,0)) == mapgen.groundTile &&
                    mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX-1, randY,0)) == mapgen.groundTile &&
                    mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX+1, randY,0)) == mapgen.groundTile &&
                    mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX, randY-1,0)) == mapgen.groundTile &&
                    mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(randX, randY+1,0)) == mapgen.groundTile &&
                    Vector2.Distance(player.transform.position, new Vector2(randX,randY)) > 10 && 
                    !enemies.Any(e => e.transform.position == new Vector3(randX, randY,0)))
                { break; }
            }
            Instantiate(enemy, new Vector2(randX, randY), Quaternion.identity);
            enemies.Add(enemy);
        }
    }    
}
