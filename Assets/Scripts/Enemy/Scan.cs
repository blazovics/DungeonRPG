using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scan : MonoBehaviour

{
    float start;
    bool scanned;

    // Start is called before the first frame update
    public void Start()
    {
        start = Time.time;
        scanned = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time> start + 0.1 && !scanned)
        {
            AstarPath.active.Scan();
            scanned = true;
        }
    }
}
