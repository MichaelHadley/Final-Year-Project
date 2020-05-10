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
    public static string bestLapMinutes;
    public static string bestLapSeconds;
    public static string bestLapMilliseconds;

    public static string pos1Name;
    public static string pos2Name;
    public static string pos3Name;
    public static string pos4Name;
    public static string pos5Name;

    public GameObject pos1NameBox;
    public GameObject pos2NameBox;
    public GameObject pos3NameBox;
    public GameObject pos4NameBox;
    public GameObject pos5NameBox;

    public GameObject minuteBox;
    public GameObject secondBox;
    public GameObject milliSecondBox;

    public GameObject minuteBoxBest;
    public GameObject secondBoxBest;
    public GameObject milliSecondBoxBest;

    public static float gameTime;


    private void Start()
    {
        gameTime = 0;
        millisecondCount = 0;

        bestLapMinutes = "00:";
        bestLapSeconds = "00.";
        bestLapMilliseconds = "0";
        minuteBoxBest.GetComponent<Text>().text = bestLapMinutes;
        secondBoxBest.GetComponent<Text>().text = bestLapSeconds;
        milliSecondBoxBest.GetComponent<Text>().text = bestLapMilliseconds;

        pos1Name = "-";
        pos2Name = "-";
        pos3Name = "-";
        pos4Name = "-";
        pos5Name = "-";
        pos1NameBox.GetComponent<Text>().text = pos1Name;
        pos2NameBox.GetComponent<Text>().text = pos2Name;
        pos3NameBox.GetComponent<Text>().text = pos3Name;
        pos4NameBox.GetComponent<Text>().text = pos4Name;
        pos5NameBox.GetComponent<Text>().text = pos5Name;

    }

    // Update is called once per frame
    void Update()
    {
        millisecondCount += Time.deltaTime * 10;
        gameTime += Time.deltaTime;
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

        //Update BestTime
        minuteBoxBest.GetComponent<Text>().text = bestLapMinutes;
        secondBoxBest.GetComponent<Text>().text = bestLapSeconds;
        milliSecondBoxBest.GetComponent<Text>().text = bestLapMilliseconds;


        //Update Positions
        pos1NameBox.GetComponent<Text>().text = pos1Name;
        pos2NameBox.GetComponent<Text>().text = pos2Name;
        pos3NameBox.GetComponent<Text>().text = pos3Name;
        pos4NameBox.GetComponent<Text>().text = pos4Name;
        pos5NameBox.GetComponent<Text>().text = pos5Name;
    }
}
