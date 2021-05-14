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

    public int width = 30;
    public int wall_width = 2;
    public static int smooth8 = 3;
    public static int smooth4 = 3;

    // Start is called before the first frame update
    void Start()
    {
        Map map = new Map(4, 10, tiles);
        map.rooms[0, 3].drawRoom();
    }

    class Room {
        int x;
        int y;
        int width;
        int wall_width;
        int[,] data;
        public bool[] doors;
        GameObject[] tiles;

        public Room(int x, int y, GameObject[] tiles, bool[] doors = null, int width = 30, int wall_width = 2) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.wall_width = wall_width;
            this.tiles = tiles;
            
            this.doors = doors;
            if (this.doors == null)
                this.doors = new bool[4]{true, true, true, true};

            this.data = new int[width,width];

            init();
        }

        private void init() {
            generateRoom();
            smoothRoom8(smooth8);
            smoothRoom4(smooth4);
            //drawRoom();
        }

        // DEBUG ONLY DONT USE IT
        public void drawRoom()
        {
            for (int y = width-1; y >= 0; y--)
                for (int x = 0; x < width; x++)
                    GameObject.Instantiate(tiles[data[y, x]], new Vector3(y-width/2, x-width/2, 0), Quaternion.identity);
        }

        private void generateRoom()
        {
            for (int y = 0; y < width; y++) {
                for (int x = 0; x < width; x++) {
                    if (y < wall_width || y > width-1-wall_width || x < wall_width || x > width-1-wall_width) {
                        data[y, x] = 1;
                    }
                    else {
                        data[y, x] = Random.Range(0, 2);
                    }
                }
            }

            for (int y = 0; y < width; y++) {
                for (int x = 0; x < width; x++) {
                    if (x > (int)(width/8*4)-1 && x < (int)(width/8*6)) {
                        if (y < (int)width/5) {
                            if (doors[0]) {
                                data[y, x] = 0;
                            }
                            if (doors[3]) {
                                data[x, y] = 0;
                            }
                        }
                        if (y > (int)width/5*4) {
                            if (doors[2]) {
                                data[y, x] = 0;
                            }
                            if (doors[1]) {
                                data[x, y] = 0;
                            }
                        }
                    }
                }
            }
        }

        // 4 szomszedot nez
        int getPointValue4(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= width)
            {
                Debug.Log("Index out of bounds: " + x + ":" + y);
                return -1;
            }

            int value = 0;

            for (int y_offset = -1; y_offset < 2; y_offset++) {
                for (int x_offset = -1; x_offset < 2; x_offset++) {
                    int xx = x + x_offset;
                    int yy = y + y_offset;
                    if (xx >= 0 && xx < width && yy >= 0 && yy < width) {
                        if (x_offset != y_offset) {
                            if (data[yy, xx] == 1) {
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

            if (x < 0 || x >= width || y < 0 || y >= width) {
                Debug.Log("Index out of bounds: " + x + ":" + y);
                return -1;
            }

            int value = 0;

            for (int y_offset = -1; y_offset < 2; y_offset++) {
                for (int x_offset = -1; x_offset < 2; x_offset++) {
                    int xx = x + x_offset;
                    int yy = y + y_offset;
                    if (xx >= 0 && xx < width && yy >= 0 && yy < width) {
                        if (x_offset != 0 || y_offset != 0){
                            if (data[yy, xx] == 1) {
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
                int[, ] newRoom = new int[width, width];
                for (int y = 0; y < width; y++) {
                    for (int x = 0; x < width; x++) {
                        newRoom[y, x] = -1;
                    }
                }

                // vegig megyunk az adott szoban
                for (int y = 0+wall_width; y < width-wall_width; y++) {
                    for (int x = 0+wall_width; x < width-wall_width; x++) {
                        if (getPointValue8(x, y) >= 5) {
                            if (data[y, x] != 1) {
                                newRoom[y, x] = 1;
                            }
                        }
                        else {
                            if (data[y, x] != 0) {
                                newRoom[y, x] = 0;
                            }
                        }
                    }
                }

                // valtozasokat visszairjuk az eredetibe
                for (int y = 0; y < width; y++) {
                    for (int x = 0; x < width; x++) {
                        if (newRoom[y, x] != -1) {
                            data[y, x] = newRoom[y, x];
                        }
                    }
                }
            }
        }

        void smoothRoom4(int times = 1) {
            for (int i = 0; i < times; i++) {
                // uj ures szoba amibe irjuk a valtozasokat
                int[, ] newRoom = new int[width, width];
                for (int y = 0; y < width; y++) {
                    for (int x = 0; x < width; x++) {
                        newRoom[y, x] = -1;
                    }
                }

                // vegig megyunk az adott szoban
                for (int y = 0+wall_width; y < width-wall_width; y++) {
                    for (int x = 0+wall_width; x < width-wall_width; x++) {
                        if (getPointValue4(x, y) < 2 && data[y, x] == 1) 
                            newRoom[y, x] = 0;
                        if (getPointValue4(x, y) > 3 && data[y, x] == 0)
                            newRoom[y, x] = 1;
                    }
                }

                // valtozasokat visszairjuk az eredetibe
                for (int y = 0; y < width; y++) {
                    for (int x = 0; x < width; x++) {
                        if (newRoom[y, x] != -1) {
                            data[y, x] = newRoom[y, x];
                        }
                    }
                }
            }
        }
    }

    class Map {
        int width;
        int max_rooms;
        public Room[,] rooms;
        List<(int x, int y)> path;
        GameObject[] tiles;
        
        public Map(int width, int max_rooms, GameObject[] tiles) {
            this.width = width;
            this.max_rooms = max_rooms;
            this.rooms = new Room[width, width];
            this.path = new List<(int x, int y)>();
            this.tiles = tiles;

            init();
            makePath();
        }

        private void init() {
            for (int y = 0; y < width; y++) {
                for (int x = 0; x < width; x++) {
                    var doors = new bool[4]{Random.Range(0, 2) == 1, Random.Range(0, 2) == 1, Random.Range(0, 2) == 1, Random.Range(0, 2) == 1};
                    if (y == 0) {
                        doors[0] = false;
                    } else if (y == width-1) {
                        doors[2] = false;
                    }
                    if (x == 0) {
                        doors[3] = false;
                    } else if (x == width-1) {
                        doors[1] = false;
                    }
                    rooms[y, x] = new Room(x, y, tiles, doors);
                }
            }
        }

        private void makePath() {
            int spawnPoint = (int)(width/2);
            int dir = Random.Range(1, 5);
            int level = 0;
            int nIndex = 0;
            int currentX = spawnPoint;
        }
    }
}