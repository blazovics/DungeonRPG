using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generation : MonoBehaviour
{
    public GameObject[] tiles = new GameObject[10];
    public Tilemap wallTilemap;
    public Tilemap groundTilemap;
    public WeightedRandomTile wallTile;
    public RandomTile groundTile;
    public GameObject exit;
    public Map map;

    public int width = 30;
    public int wall_width = 2;
    public static int smooth8 = 4;
    public static int smooth4 = 4;

    // Start is called before the first frame update
    public void Start()
    {
        map = new Map(5, 10, tiles, wallTilemap, groundTilemap, groundTile, wallTile, exit);
        map.drawWholeMap();
        
        //print(map.GetStartingRoom().x + " " + map.GetStartingRoom().y);
        //map.rooms[0, 0].drawRoom();
    }

    public class Room {
        public int x;
        public int y;
        public int width;
        public int wall_width;
        public int[,] data;
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
            for (int y = 0; y < width; y++)
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

    public class Map {
        int width;
        int max_rooms;
        public Room[,] rooms;
        public List<(int x, int y)> path;
        GameObject[] tiles;
        Tilemap wallTilemap;
        Tilemap groundTilemap;
        WeightedRandomTile wallTile;
        RandomTile groundTile;
        GameObject exit;

        public Tilemap GetWallTilemap()
        {
            return wallTilemap;
        }

        public Tilemap GetGroundTilemap()
        {
            return groundTilemap;
        }
        
        public void SetTilemapToNull()
        {
            wallTilemap = GetWallTilemap();
            groundTilemap = GetGroundTilemap();
            //rint(wallTilemap.cellBounds);
            for (int x = -74; x < 75; x++)
            {
                for (int y = -74; y < 75; y++)
                {
                    //print("Done");
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    //print(x + " " + y);
                }
            }            
            /*
            for (int x = -74; x < 75; x++)
            {
                for (int y = -74; y < 75; y++)
                {
                    print(wallTilemap.GetTile(new Vector3Int(x, y, 0)));
                }
            }
            */
        }
        

        public Room GetStartingRoom()
        {
            foreach (var item in rooms)
            {
                if (path[0].x == item.x && path[0].y == item.y)
                {
                    return item;
                }
            }
            return null;         
        }

        public Room GetFinalRoom()
        {
            foreach (var item in rooms)
            {
                if (path[path.Count-1].x == item.x && path[path.Count - 1].y == item.y)
                {
                    //print(item.x + "halge" + item.y);
                    return item;
                }
            }
            return null;
        }


        public Map(int width, int max_rooms, GameObject[] tiles, Tilemap wallTilemap, Tilemap groundTilemap, RandomTile groundTile, WeightedRandomTile wallTile, GameObject exit) {
            this.width = width;
            this.max_rooms = max_rooms;
            this.rooms = new Room[width, width];
            this.tiles = tiles;
            this.wallTilemap = wallTilemap;
            this.groundTilemap = groundTilemap;
            this.groundTile = groundTile;
            this.wallTile = wallTile;
            this.exit = exit;

            init();
            makePath();
        }

        public void drawWholeMap() {
            int max_width = width * rooms[0, 0].width;
            Room finalRoom = GetFinalRoom();
            bool spawnedExit = true;
            for (int y = 0; y < max_width; y++) {
                for (int x = 0; x < max_width; x++) {
                    //print(rooms[(int)(y / 30), (int)(x / 30)].data[(int)(y % 30), (int)(x % 30)]);
                    //GameObject.Instantiate(tiles[rooms[(int)(y/30), (int)(x/30)].data[(int)(y%30), (int)(x%30)]], new Vector3(y-max_width/2, x-max_width/2, 0), Quaternion.identity);                    
                    if (rooms[(int)(y / 30), (int)(x / 30)].data[(int)(y % 30), (int)(x % 30)] == 0 && finalRoom.x == rooms[(int)(y / 30), (int)(x / 30)].x && finalRoom.y == rooms[(int)(y / 30), (int)(x / 30)].y && spawnedExit)
                    {
                        groundTilemap.SetTile(new Vector3Int(y - max_width / 2, x - max_width / 2, 0), groundTile);
                        GameObject.Instantiate(exit , new Vector3((y - max_width / 2) + 1.0f, (x - max_width / 2) + 1.0f, 0), Quaternion.identity);
                        spawnedExit = false;
                    }
                    else if (rooms[(int)(y / 30), (int)(x / 30)].data[(int)(y % 30), (int)(x % 30)] == 0)
                    {
                        groundTilemap.SetTile(new Vector3Int(y - max_width / 2, x - max_width / 2, 0), groundTile);                        
                    }
                    else
                    {
                        wallTilemap.SetTile(new Vector3Int(y - max_width / 2, x - max_width / 2, 0), wallTile);
                    }
                }
            }


            /*
            foreach (var item in rooms)
            {
                print(item.x + " " + item.y);
            }            
            foreach (var item in rooms)
            {
                //print("asdad" + path[0].x + item.x + "||" + path[0].y + item.y);
                if (path[0].x == item.x && path[0].y == item.y)
                {
                    print(item.x + "szoba" + item.y);
                    print(path[0].x + "path" + path[0].y);
                }
            }
            */

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

        // helper func
        int randomChoice(int[] list) {
            return list[Random.Range(0, list.Length)];
        }

        private void makePath() {
            int spawnPoint = (int)(width/2);
            int dir = Random.Range(1, 5);
            int level = 0;
            int nIndex = 0;
            int currentX = spawnPoint;
            int prevDir = -1;
            bool[] doors = new[]{true, true, true, true};
            path = new List<(int x, int y)>();

            while (level < width-1 && max_rooms > nIndex) {

                //redirecting before using wrong values
                if (new List<int>(){1, 2}.Contains(prevDir) && new List<int>(){3, 4}.Contains(dir)) {
                    dir = randomChoice(new[]{1, 2, 5});
                }
                if (new List<int>(){3, 4}.Contains(prevDir) && new List<int>(){1, 2}.Contains(dir)) {
                    dir = randomChoice(new[]{3, 4, 5});
                }

                // generating spawn room
                if (new List<int>(){1, 2}.Contains(dir)) {
                    doors = new[]{false, false, false, true};
                } else if (new List<int>(){3, 4}.Contains(dir)) {
                    doors = new[]{false, true, false, false};
                } else if (dir == 5) {
                    doors = new[]{false, false, true, false};
                }

                // decide where the doors should be
                if (new List<int>(){1, 2}.Contains(prevDir)) {
                    if (new List<int>(){1, 2}.Contains(dir)) {
                        doors = new[]{false, true, false, true};
                    } else if (dir == 5) {
                        doors = new[]{false, true, true, false};
                    }
                } else if (new List<int>(){3, 4}.Contains(prevDir)) {
                    if (new List<int>(){3, 4}.Contains(dir)) {
                        doors = new[]{false, true, false, true};
                    } else if (dir == 5) {
                        doors = new[]{false, false, true, true};
                    }
                } else if (prevDir == 5) {
                    if (new List<int>(){1, 2}.Contains(dir)) {
                        doors = new[]{true, false, false, true};
                    } else if (new List<int>(){3, 4}.Contains(dir)) {
                        doors = new[]{true, true, false, false};
                    } else {
                        doors = new[]{true, false, true, false};
                    }
                }

                // going left
                if (new List<int>(){1, 2}.Contains(dir)) {
                    if (currentX <= 0) {
                        dir = 5;
                        if (prevDir == 5) {
                            doors = new[]{true, false, true, false};
                        } else {
                            doors = new[]{false, true, true, false};
                        }
                    } else {
                        rooms[level, currentX] = new Room(currentX, level, tiles, doors);
                        currentX--;
                    }
                }

                // going right
                if (new List<int>(){3, 4}.Contains(dir)) {
                    if (currentX >= width-1) {
                        dir = 5;
                        if (prevDir == 5) {
                            doors = new[]{true, false, true, false};
                        } else {
                            doors = new[]{false, false, true, true};
                        }
                    } else {
                        rooms[level, currentX] = new Room(currentX, level, tiles, doors);
                        currentX++;
                    }
                }

                // going down
                if (dir == 5) {
                    rooms[level, currentX] = new Room(currentX, level, tiles, doors);
                    level++;
                }

                // end of while
                prevDir = dir;
                dir = Random.Range(1, 5);
                nIndex++;
                path.Add((currentX, level));
            }

            // generating end room
            if (new List<int>(){1, 2}.Contains(prevDir)) {
                doors = new[]{false, true, false, false};
            } else if (new List<int>(){3, 4}.Contains(prevDir)) {
                doors = new[]{false, false, false, true};
            } else if (prevDir == 5) {
                doors = new[]{true, false, false, false};
            }
            rooms[level, currentX] = new Room(currentX, level, tiles, doors);
            path.Add((currentX, level));

            for (int i = 0; i < path.Count; i++)
            {
                //print(path[i]);
            }
        }
    }
}