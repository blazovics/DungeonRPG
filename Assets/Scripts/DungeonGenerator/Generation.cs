using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Generation : MonoBehaviour
{
    GameObject tile;
    GameObject[,] tileArray;

    public GameObject[] Tiles = new GameObject[100];

    int[,] arrayList;
    int height;
    int width;
    int x = 0;
    int y = 0;
    void readData()
    {
        arrayList = new int[,]{ 
            {1, 1, 1, 1, 1, 1, 1, 1, 1 },
            {1, 3, 0, 0, 0, 0, 0, 0, 1 }, 
            {1, 0, 0, 0, 0, 0, 0, 0, 1 }, 
            {1, 0, 0, 0, 0, 0, 0, 0, 1 }, 
            {1, 0, 0, 0, 0, 0, 0, 2, 1 },
            {1, 0, 0, 0, 0, 0, 0, 0, 1 }, 
            {1, 0, 0, 0, 0, 0, 0, 0, 1 }, 
            {1, 1, 1, 1, 1, 1, 1, 1, 1 },
        };

        height = arrayList.GetLength(0);
        width = arrayList.GetLength(1);
    }

    void DrawMap()
    {
        tileArray = new GameObject[(height*100), (width*100)];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                print(i);
                print(j);
                if (arrayList[i,j] == 1)
                {
                    tileArray[x, y] = Tiles[0];
                    tile = tileArray[x, y];
                    GameObject.Instantiate(tile, new Vector3(i, j, 0), Quaternion.identity);                    
                }
                else if (arrayList[i, j] == 0)
                {
                    tileArray[x, y] = Tiles[1];
                    tile = tileArray[x, y];
                    GameObject.Instantiate(tile, new Vector3(i, j, 0), Quaternion.identity);                   
                }
                else if (arrayList[i, j] == 2 || arrayList[i, j] == 3 )
                {
                    tileArray[x, y] = Tiles[2];
                    tile = tileArray[x, y];
                    GameObject.Instantiate(tile, new Vector3(i, j, 0), Quaternion.identity);                    
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        readData();
        DrawMap();
    }

}
