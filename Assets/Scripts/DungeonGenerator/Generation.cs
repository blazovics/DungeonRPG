using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Generation : MonoBehaviour
{
    GameObject tile;
    GameObject[,] tileArray;

    public GameObject[] tiles = new GameObject[10];

    int[,] room;
    public int height = 30;
    public int width = 30;
    public int wall_width = 2;
    public int smooth8 = 3;
    public int smooth4 = 3;

    // 4 szomszedot nez
    int getPointValue4(int x, int y) {

        if (x < 0 || x >= width || y < 0 || y >= height) {
            Debug.Log("Index out of bounds: " + x + ":" + y);
            return -1;
        }

        int value = 0;

        for (int y_offset = -1; y_offset < 2; y_offset++) {
            for (int x_offset = -1; x_offset < 2; x_offset++) {
                int xx = x + x_offset;
                int yy = y + y_offset;
                if (xx >= 0 && xx < width && yy >= 0 && yy < height) {
                    if (x_offset != y_offset) {
                        if (room[yy, xx] == 1) {
                            value += 1;
                        }
                    }
                }
            }
        }

        return value;
    }

    // 8 szomszedot nez
    int getPointValue8(int x, int y) {

        if (x < 0 || x >= width || y < 0 || y >= height) {
            Debug.Log("Index out of bounds: " + x + ":" + y);
            return -1;
        }

        int value = 0;

        for (int y_offset = -1; y_offset < 2; y_offset++) {
            for (int x_offset = -1; x_offset < 2; x_offset++) {
                int xx = x + x_offset;
                int yy = y + y_offset;
                if (xx >= 0 && xx < width && yy >= 0 && yy < height) {
                    if (x_offset != 0 || y_offset != 0){
                        if (room[yy, xx] == 1) {
                            value += 1;
                        }
                    }
                }
            }
        }

        return value;
    }

    void smoothRoom8(int times = 1) {
        for (int i = 0; i < times; i++) {
            // uj ures szoba amibe irjuk a valtozasokat
            int[, ] newRoom = new int[height, width];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    newRoom[y, x] = -1;
                }
            }

            // vegig megyunk az adott szoban
            for (int y = 0+wall_width; y < height-wall_width; y++) {
                for (int x = 0+wall_width; x < width-wall_width; x++) {
                    if (getPointValue8(x, y) >= 5) {
                        newRoom[y, x] = 1;
                    }
                    else {
                        newRoom[y, x] = 0;
                    }
                }
            }

            // valtozasokat visszairjuk az eredetibe
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (newRoom[y, x] != -1) {
                        room[y, x] = newRoom[y, x];
                    }
                }
            }
        }
    }

    void smoothRoom4(int times = 1) {
        for (int i = 0; i < times; i++) {
            // uj ures szoba amibe irjuk a valtozasokat
            int[, ] newRoom = new int[height, width];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    newRoom[y, x] = -1;
                }
            }

            // vegig megyunk az adott szoban
            for (int y = 0+wall_width; y < height-wall_width; y++) {
                for (int x = 0+wall_width; x < width-wall_width; x++) {
                    if (getPointValue4(x, y) < 2 && room[y, x] == 1) 
                        newRoom[y, x] = 0;
                    if (getPointValue4(x, y) > 3 && room[y, x] == 0)
                        newRoom[y, x] = 1;
                }
            }

            // valtozasokat visszairjuk az eredetibe
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (newRoom[y, x] != -1) {
                        room[y, x] = newRoom[y, x];
                    }
                }
            }
        }
    }

    void generateRoom()
    {
        room = new int[height, width];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (y < wall_width || y > height-1-wall_width || x < wall_width || x > width-1-wall_width) {
                    room[y, x] = 1;
                }
                else {
                    room[y, x] = Random.Range(0, 2);
                }
            }
        }
    }

    void drawRoom()
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                GameObject.Instantiate(tiles[room[y, x]], new Vector3(y-height/2, x-width/2, 0), Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start()
    {
        generateRoom();
        smoothRoom8(smooth8);
        smoothRoom4(smooth4);
        drawRoom();
    }
}
