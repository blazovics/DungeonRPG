using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Counts down on the screen
public class Countdown : MonoBehaviour
{
    //singleton
    public static Countdown instance { get; private set; }

    private bool isCountingDown = false;
    private float currentTime = 0.0f;
    private float startingTime;

    public Text countdownText;
    private void Awake()
    {
        instance = this;
        isCountingDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCountingDown)
            return;

        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");

        if (currentTime <= 0)
        {
            currentTime = 0.0f;
            isCountingDown = false;
        }
    }

    public void StartCountdown(float s)
    {
        startingTime = s;
        currentTime = startingTime;
        isCountingDown = true;
    }

}
