using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generation : MonoBehaviour
{
    GameObject tile;
    GameObject[,] tileArray;

    public GameObject[] tiles = new GameObject[10];

    public Tilemap level;
    public Tile wallTile;
    public Tile groundTile;
    public AnimatedTile torch;
    public int[,] room;
    public int height = 30;
    public int width = 30;
    public int wall_width = 2;
    public int smooth8 = 3;
    public int smooth4 = 3;
    public Map palya = new Map(1, 1, 5);
    


    public class Room
    {
        public int x;
        public int y;
        public bool[] doors;
        public int[,] data = new int[30,30];
        public Room(int x, int y, bool[] doors = null)
        {
            this.x = x;
            this.y = y;
            this.doors = doors;          

            if (doors == null)
            {
                for (int i = 0; i < doors.Length; i++)
                {
                    doors[i] = true;
                }
            }            
        }
    }

    public class Map
    {        
        public int y_max = 5;
        public int x_max = 5;
        public int max_rooms = 5;
        public Room[,] rooms = new Room[5, 5];
        public List<(int,int)> path = new List<(int, int)>();
        public int[,] minimap = new int[5, 5];

        public Map(int x_max,int y_max,int max_rooms)
        {
            this.x_max = x_max;
            this.y_max = y_max;
            this.max_rooms = max_rooms;

        }

        public void generateMap()
        {
            for (int x = 0; x < x_max; x++)
            {
                for (int y = 0; y < y_max; y++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                    //Potenci�lis hiba, true false felcser�l�se
                        rooms[x, y].doors[i] = Random.Range(0, 2) == 0 ? true : false;
                    }

                    if (y == 0)
                    {
                        rooms[x, y].doors[0] = false;
                    }
                    else if (y == y_max - 1)
                    {
                        rooms[x, y].doors[2] = false;
                    }
                    if (x == 0)
                    {
                        rooms[x, y].doors[3] = false;
                    }
                    else if (x == x_max - 1)
                    {
                        rooms[x, y].doors[1] = false;
                    }
                }
            }
            makePath();
        }
        public void makePath()
        {
            int spawnPoint = (int)(this.minimap.Length / 2);
            int direction = Random.Range(1, 5);
            int level = 0;
            int nIndex = 0;
            int currentX = spawnPoint;
            int prevDirection = -1;
            int[] randarr1 = new int[] { 1, 2 };
            int[] randarr2 = new int[] { 3, 4 };

            Room currentRoom = new Room(currentX, level);

            int[] dir1 = new int[] { 1, 2, 5 };
            System.Random r = new System.Random();

            //potenci�lis hiba : minimap kihagyva
            //generation every room
            while (this.max_rooms > nIndex)
            {
                //redirecting before using wrong values
                if ((prevDirection == 1 || prevDirection == 2) && (direction == 3 || direction == 4))
                {
                    int rIndex = r.Next(dir1.Length);
                    direction = dir1[rIndex];
                }
                if ((prevDirection == 3 || prevDirection == 4) && (direction == 1 || direction == 2))
                {
                    direction = Random.Range(3, 6);
                }
                //generating spawn room
                if (direction == 1 || direction == 2)
                {
                    currentRoom = new Room(currentX, level, new bool[] { false, false, false, true });
                }
                else if (direction == 3 || direction == 4)
                {
                    currentRoom = new Room(currentX, level, new bool[] { false, true, false, false });
                }
                else if (direction == 5)
                {
                    currentRoom = new Room(currentX, level, new bool[] { false, false, true, false });
                }

                //decide where the doors should be

                if (prevDirection == 1 || prevDirection == 2)
                {
                    if (direction == 1 || direction == 2)
                    {
                        currentRoom = new Room(currentX, level, new bool[] { false, true, false, true });
                    }
                    else if (direction == 5)
                    {
                        currentRoom = new Room(currentX, level, new bool[] { false, true, true, false });
                    }
                }
                else if (prevDirection == 3 || prevDirection == 4)
                {
                    if (direction == 3 || direction == 4)
                    {
                        currentRoom = new Room(currentX, level, new bool[] { false, true, false, true });
                    }
                    else if (direction == 5)
                    {
                        currentRoom = new Room(currentX, level, new bool[] { false, false, true, true });
                    }
                }
                else if (prevDirection == 5)
                {
                    if (direction == 1 || direction == 2)
                    {
                        currentRoom = new Room(currentX, level, new bool[] { true, false, false, true });
                    }
                    else if (direction == 3 || direction == 4)
                    {
                        currentRoom = new Room(currentX, level, new bool[] { true, true, false, false });
                    }
                    else
                    {
                        currentRoom = new Room(currentX, level, new bool[] { true, false, true, false });
                    }
                }
                //going left
                if (randarr1.Contains(direction))
                {
                    if (currentX <= 0)
                    {
                        direction = 5;
                        if (prevDirection == 5)
                        {
                            currentRoom = new Room(currentX, level, new bool[] { true, false, true, false });
                        }
                        else
                        {
                            currentRoom = new Room(currentX, level, new bool[] { false, true, true, false });
                        }
                    }
                    else
                    {
                        this.rooms[level, currentX] = currentRoom;
                        currentX += 1;
                    }
                }
                //going right 
                if (randarr2.Contains(direction))
                {
                    if (currentX >= this.minimap.Length - 1)
                    {
                        direction = 5;
                        if (prevDirection == 5)
                        {
                            currentRoom = new Room(currentX, level, new bool[] { true, false, true, false });
                        }
                        else
                        {
                            currentRoom = new Room(currentX, level, new bool[] { false, false, true, true });
                        }
                    }
                    else
                    {
                        this.rooms[level, currentX] = currentRoom;
                        currentX += 1;
                    }
                }
                //going down 
                if (direction == 5)
                {
                    this.rooms[level, currentX] = currentRoom;
                    level += 1;
                }
                //end of while
                prevDirection = direction;
                direction = Random.Range(1, 6);
                nIndex += 1;
                this.path.Add((currentX, level));
            }
            //generating end room
            if (randarr1.Contains(prevDirection))
            {
                currentRoom = new Room(currentX, level, new bool[] { false, true, false, false });
            }
            else if (randarr2.Contains(prevDirection))
            {
                currentRoom = new Room(currentX, level, new bool[] { false, false, false, true });
            }
            else if (prevDirection == 5)
            {
                currentRoom = new Room(currentX, level, new bool[] { true, false, false, false });
            }
            this.rooms[level, currentX] = currentRoom;
            this.path.Add((currentX, level));
        }
    }

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
                if (room[y,x] == 1)
                {
                    //GameObject.Instantiate(tiles[1], new Vector3(y - height / 2, x - width / 2, 0), Quaternion.identity);
                    level.SetTile(new Vector3Int(y - height / 2, x - width / 2, 0), torch);
                }
                else if (room[y, x] == 0)
                {
                    //GameObject.Instantiate(tiles[0], new Vector3(y - height / 2, x - width / 2, 0), Quaternion.identity);
                    level.SetTile(new Vector3Int(y - height / 2, x - width / 2, 0), groundTile);
                }
    }
    /*
    void clearMap(Map palya)
    {
        for (int y = 0; y < palya.x_max; y++)
        {
            for (int x = 0; x < palya.y_max; x++)
            {
                palya.rooms[y,x].r
            }
        }
    }
    */
    public void drawWholeMap(Room[,] rooms)
    {
        print("Elkezdtem");
        int x_max = rooms.Length * 30;
        int y_max = rooms.Length * 30;
        for (int y = 0; y < y_max; y++)
        {
            for (int x = 0; x < x_max; x++)
            {
                print(x + " "+ y);
                int currentTileIndex = rooms[(int)(x / 30), (int)(y / 30)].data[x % 30, y % 30];
                if (currentTileIndex == 1)
                {
                    //GameObject.Instantiate(tiles[1], new Vector3(y - height / 2, x - width / 2, 0), Quaternion.identity);
                    int rand = Random.Range(0, 30);
                    if (rand == 0)
                    {
                        level.SetTile(new Vector3Int(y - height / 2, x - width / 2, 0), torch);
                    }
                    else if (rand == 1)
                    {
                        level.SetTile(new Vector3Int(y - height / 2, x - width / 2, 0), wallTile); //nincs meg banner
                    }
                    else
                        level.SetTile(new Vector3Int(y - height / 2, x - width / 2, 0), wallTile);
                }
                else if (currentTileIndex == 0)
                {
                    //GameObject.Instantiate(tiles[0], new Vector3(y - height / 2, x - width / 2, 0), Quaternion.identity);
                    level.SetTile(new Vector3Int(y - height / 2, x - width / 2, 0), groundTile);
                }
            }
        }
    } 

    // Start is called before the first frame update
    void Start()
    {        
        generateRoom();
        smoothRoom8(smooth8);
        smoothRoom4(smooth4);
        //drawRoom();
        drawWholeMap(palya.rooms);
    }
}
