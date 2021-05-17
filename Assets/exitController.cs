using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnightController player = collision.GetComponent<KnightController>();
        Generation tilemap;

        if (collision != null)
        {
            player.enabled = false;
            Destroy(GameObject.FindGameObjectWithTag("TileMap"));
            player.enabled = true;
        }
    }
}
