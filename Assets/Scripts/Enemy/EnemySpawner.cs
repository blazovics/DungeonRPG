using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public int level;
    public GameObject enemy;
    public GameObject enemy2;
    public GameObject enemy3;
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
            float random = Random.Range(0.0f,9.0f);
            if(random < 3.0f)
            {
                enemy.GetComponent<enemy_main_controller>().player = player;
                Instantiate(enemy, new Vector2(randX, randY), Quaternion.identity);
                enemies.Add(enemy);
            }
            if (3.0 > random && random < 6.0)
            {
                enemy2.GetComponent<enemy_main_controller>().player = player;
                Instantiate(enemy2, new Vector2(randX, randY), Quaternion.identity);
                enemies.Add(enemy2);
            }
            if (6.0 > random && random < 9.0)
            {
                enemy3.GetComponent<enemy_main_controller>().player = player;
                Instantiate(enemy3, new Vector2(randX, randY), Quaternion.identity);
                enemies.Add(enemy3);
            }

        }
    }    
}
