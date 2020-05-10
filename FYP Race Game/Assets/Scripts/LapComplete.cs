using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LapComplete : MonoBehaviour
{
    public GameObject lapCompleteTrigger;
    public GameObject halfLapTrigger;

    public GameObject minuteDisplay;
    public GameObject secondDisplay;
    public GameObject millisecondDisplay;

    public LapTimer lapTimer;
    public GameObject lapCounter;
    public int maxLaps; //This is set in Cars Manager
    public static int totalLaps;
    public  float gameTime;

    public GameObject RaceFinished;


    private void Start()
    {
        maxLaps = 3;
        totalLaps = 0;
    }
    void Update()
    {
        if(totalLaps == maxLaps)
        {
            RaceFinished.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        /*
        //totalLaps += 1;
        gameTime = PlayerPrefs.GetFloat("gameTime");
        if (gameTime > 5)
        {
            if (lapTimer.gameTime <= gameTime)
            {
                if (lapTimer.secondCount <= 9)
                {
                    secondDisplay.GetComponent<Text>().text = "0" + lapTimer.secondCount + ".";
                }
                else
                {
                    secondDisplay.GetComponent<Text>().text = "" + lapTimer.secondCount + ".";
                }

                if (lapTimer.minuteCount <= 9)
                {
                    minuteDisplay.GetComponent<Text>().text = "0" + lapTimer.minuteCount + ".";
                }
                else
                {
                    minuteDisplay.GetComponent<Text>().text = "" + lapTimer.minuteCount + ".";
                }

                millisecondDisplay.GetComponent<Text>().text = "" + lapTimer.millisecondCount;
            }
            }
          
        PlayerPrefs.SetInt("minuteSave", LapTimer.minuteCount);
        PlayerPrefs.SetInt("secondSave", LapTimer.secondCount);
        PlayerPrefs.SetFloat("millisecondSave", LapTimer.millisecondCount);
        PlayerPrefs.SetFloat("gameTime", LapTimer.gameTime);
        */


        lapCounter.GetComponent<Text>().text = "" + totalLaps;
        // halfLapTrigger.SetActive(true);
        // lapCompleteTrigger.SetActive(false);

    }
}
