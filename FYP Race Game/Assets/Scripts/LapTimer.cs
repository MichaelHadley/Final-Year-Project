using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapTimer : MonoBehaviour
{
    public static int minuteCount;
    public static int secondCount;
    public static float millisecondCount;
    public static string millisecondDisplay;

    public GameObject minuteBox;
    public GameObject secondBox;
    public GameObject milliSecondBox;

    // Update is called once per frame
    void Update()
    {
        millisecondCount += Time.deltaTime * 10; 
        millisecondDisplay = millisecondCount.ToString ("F0");  // convert to string
        milliSecondBox.GetComponent<Text>().text = "" + millisecondDisplay; //display text on screen in ui

        if(millisecondCount >= 10)
        {
            millisecondCount = 0;
            secondCount += 1;
        }

        if(secondCount <= 9)
        {
            secondBox.GetComponent<Text>().text = "0" + secondCount + ".";
        }
        else
        {
            secondBox.GetComponent<Text>().text = "" + secondCount + ".";
        }

        if(secondCount == 60)
        {
            secondCount = 0;
            minuteCount += 1;
        }

        if(minuteCount <= 9)
        {
            minuteBox.GetComponent<Text>().text = "0" + minuteCount + ":";
        }
        else
        {
            minuteBox.GetComponent<Text>().text = "" + minuteCount + ":";
        }
    }
}
