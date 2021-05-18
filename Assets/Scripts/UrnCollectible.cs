using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrnCollectible : MonoBehaviour
{
    public AudioClip collectedClip;
    //On collision, the Knight picks up the Urn
    void OnTriggerEnter2D(Collider2D other)
    {
        KnightController controller = other.GetComponent<KnightController>();
        
        if (controller != null)
        {
            controller.Urns++;
            controller.PlaySound(collectedClip);
            Destroy(gameObject);
        }
    }
}
