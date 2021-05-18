using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Text UrnCount;
    public Text SpiritboxCount;
    public KnightController knightController;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpiritboxCount.text = knightController.Spiritboxes.ToString() + "x";
        UrnCount.text = knightController.Urns.ToString() + "x";
    }
}
