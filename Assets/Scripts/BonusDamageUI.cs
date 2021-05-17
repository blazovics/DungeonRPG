using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusDamageUI : MonoBehaviour
{
    public static BonusDamageUI instance { get; private set; }

    private bool spiritboxIsOn;
    private string percentage;
    public Text bonusDamageText;

    void Awake()
    {
        instance = this;
        spiritboxIsOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!spiritboxIsOn)
            return;

        bonusDamageText.text = "Damage and Speed increased by " + percentage;
    }
    
    public void StartBonusDamageUI(float p)
    {
        percentage = string.Format("{0:P}", p);
        spiritboxIsOn = true;
    }
}
