using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public Generation mapgen;
    public GameObject player;
    Generation.Room firstRoom;

    void Start()
    {
        firstRoom = mapgen.map.GetStartingRoom(); ;
        Spawn();
    }

    void Spawn()
    {
        for (int y = 0; y < firstRoom.width; y++)
        {
            for (int x = 0; x < firstRoom.width; x++)
            {
                if (firstRoom.data[y,x] == 0 && getPointValue8(y,x))
                {
                    player.transform.position = new Vector3(-59 + 30 *firstRoom.y, -72 + 30* firstRoom.x, 0);                    
                }
            }
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
