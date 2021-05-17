using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritboxCollectible : MonoBehaviour
{
    //On collision, the Knight picks up the Spiritbox
    void OnTriggerEnter2D(Collider2D other)
    {
        KnightController controller = other.GetComponent<KnightController>();

        if (controller != null)
        {
            controller.Spiritboxes++;
            Debug.Log("Current Spiritboxes: " + controller.Spiritboxes);
            Destroy(gameObject);
        }
    }
}
