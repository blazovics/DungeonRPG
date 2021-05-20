using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public Generation mapgen;
    public GameObject player;
    Generation.Room firstRoom;
    int randX, randY;
    int counter;

    public void Start()
    {
        firstRoom = mapgen.map.GetStartingRoom(); ;
        Spawn();
    }

    public void Spawn()
    {
        while (counter < 1000)
        {
            randX = UnityEngine.Random.Range(0, 29);
            randY = UnityEngine.Random.Range(29 * firstRoom.x, 29 * (firstRoom.x + 1));

            if (mapgen.map.GetGroundTilemap().GetTile(new Vector3Int(-74 + randX, -74 + randY, 0)) && getPointValue8(randX, randY%30))
            {
                player.transform.position = new Vector3(-74 + randX, -74 + randY, 0);
                break;
            }
            counter++;
        }
    }

    bool getPointValue8(int x, int y)
    {

        if (x < 0 || x >= firstRoom.width || y < 0 || y >= firstRoom.width)
        {
            Debug.Log("Index out of bounds: " + x + ":" + y);
        }

        int value = 0;

        for (int y_offset = -1; y_offset < 2; y_offset++)
        {
            for (int x_offset = -1; x_offset < 2; x_offset++)
            {
                int xx = x + x_offset;
                int yy = y + y_offset;
                if (xx >= 0 && xx < firstRoom.width && yy >= 0 && yy < firstRoom.width)
                {
                    if (x_offset != 0 || y_offset != 0)
                    {
                        if (firstRoom.data[yy, xx] == 0)
                        {
                            value += 1;
                        }
                    }
                }
            }
        }

        if (value == 8)
        {
            return true;
        }
        return false;
    }
}
