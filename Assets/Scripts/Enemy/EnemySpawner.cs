using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public int level;
    public GameObject enemy;
    public GameObject player;
    public Generation mapgen;
    int randX;
    int randY;

    void Start()
    {
        level = 2;
        Spawning();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawning()
    {
        for(int i = 0; i < level*2; i++)
        {
            while (true)
            {
                randX = UnityEngine.Random.Range(-14, 15);
                randY = UnityEngine.Random.Range(-14, 15);
                if (mapgen.level.GetTile(new Vector3Int(randX,randY,0)) == mapgen.groundTile && 
                    mapgen.level.GetTile(new Vector3Int(randX-1, randY,0)) == mapgen.groundTile &&
                    mapgen.level.GetTile(new Vector3Int(randX+1, randY,0)) == mapgen.groundTile &&
                    mapgen.level.GetTile(new Vector3Int(randX, randY-1,0)) == mapgen.groundTile &&
                    mapgen.level.GetTile(new Vector3Int(randX, randY+1,0)) == mapgen.groundTile &&
                    Vector2.Distance(player.transform.position, new Vector2(randX,randY)) > 5 )
                { break; }
            }
            Instantiate(enemy, new Vector2(randX, randY), Quaternion.identity);
        }
    }
}
