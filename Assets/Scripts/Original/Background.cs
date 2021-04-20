using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] tile;
    public Vector3 tileStartPos;
    Vector2 tileSpacing;
    public int gridWidth;
    public int gridHeight;

    private void Start()
    {
        tileSpacing = tile[0].GetComponent<Renderer>().bounds.size;

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                int randomTile = Random.Range(0, tile.Length);

                GameObject go = Instantiate(tile[randomTile], new Vector3(tileStartPos.x + (j * tileSpacing.x), tileStartPos.y + (i * tileSpacing.y)), Quaternion.identity) as GameObject;

                go.transform.parent = GameObject.Find("Background").transform;
            }
        }
    }
}
