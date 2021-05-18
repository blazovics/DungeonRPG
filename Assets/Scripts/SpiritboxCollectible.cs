using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritboxCollectible : MonoBehaviour
{
    public AudioClip collectedClip;
    //On collision, the Knight picks up the Spiritbox
    void OnTriggerEnter2D(Collider2D other)
    {
        KnightController controller = other.GetComponent<KnightController>();

        if (controller != null)
        {
            controller.Spiritboxes++;
            controller.PlaySound(collectedClip);
            Destroy(gameObject);
        }
    }
}
