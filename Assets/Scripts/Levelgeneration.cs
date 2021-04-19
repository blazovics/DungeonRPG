using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levelgeneration : MonoBehaviour
{
    public Transform[] startPos;
    public GameObject[] rooms;

    private int direction;
    public float move;

    private float timer;
    public float startTimer = 0.25f;

    public float minX;
    public float maxX;
    public float minY;
    private bool finished;

    public LayerMask room;
    // Start is called before the first frame update
    private void Start()
    {
        int randStart = Random.Range(0, startPos.Length);
        transform.position = startPos[randStart].position;
        Instantiate(rooms[0], transform.position, Quaternion.identity);

        direction = Random.Range(1, 6);
    }

    private void Update()
    {
        if (timer <= 0 && finished == false)
        {
            Move();
            timer = startTimer;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    private void Move()
    {
        Vector2 newPos;
        switch (direction)
        {
            case (1):
            case (2):
                if (transform.position.x <maxX)
                {
                    newPos = new Vector2(transform.position.x + move, transform.position.y);
                    transform.position = newPos;

                    int rand = Random.Range(0, rooms.Length);
                    Instantiate(rooms[rand], transform.position, Quaternion.identity);

                    direction = Random.Range(1, 6);

                    if (direction == 3)
                    {
                        direction = 2;
                    }
                    else if (direction == 4)
                    {
                        direction = 5;
                    }
                }
                else
                {
                    direction = 5;
                }
                break;
            case (3):
            case (4):
                if (transform.position.x > minX)  
                {
                    newPos = new Vector2(transform.position.x - move, transform.position.y);
                    transform.position = newPos;

                    direction = Random.Range(3, 6);
                }
                else
                {
                    direction = 5;
                }
                break;
            case (5):
                if (transform.position.y > minY)
                {
                    Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1 , room);
                    if (roomDetection.GetComponent<RoomType>().type != 1 && roomDetection.GetComponent<RoomType>().type != 3)
                    {
                        roomDetection.GetComponent<RoomType>().DestroyRoom();

                        int randBottomRoom = Random.Range(1, 4);
                        if (randBottomRoom == 2)
                        {
                            randBottomRoom = 1;
                        }
                        Instantiate(rooms[randBottomRoom], transform.position, Quaternion.identity);
                    }

                    newPos = new Vector2(transform.position.x, transform.position.y - move);
                    transform.position = newPos;

                    int rand = Random.Range(2, 4);
                    Instantiate(rooms[rand], transform.position, Quaternion.identity);

                    direction = Random.Range(1, 6);
                }
                else
                {
                    finished = true;
                }                
                break;
        }
    }
}
