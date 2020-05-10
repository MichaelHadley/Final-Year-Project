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

    public GameObject lapCounter;
    public int totalLaps;
    public float gameTime;

    public GameObject RaceFinished;

    void Update()
    {
        if(totalLaps == 2)
        {
            RaceFinished.SetActive(true);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        totalLaps += 1;
        gameTime = PlayerPrefs.GetFloat("gameTime");
        if (gameTime > 5)
        {
            if (LapTimer.gameTime <= gameTime)
            {
                if (LapTimer.secondCount <= 9)
                {
                    secondDisplay.GetComponent<Text>().text = "0" + LapTimer.secondCount + ".";
                }
                else
                {
                    secondDisplay.GetComponent<Text>().text = "" + LapTimer.secondCount + ".";
                }

                if (LapTimer.minuteCount <= 9)
                {
                    minuteDisplay.GetComponent<Text>().text = "0" + LapTimer.minuteCount + ".";
                }
                else
                {
                    minuteDisplay.GetComponent<Text>().text = "" + LapTimer.minuteCount + ".";
                }

                millisecondDisplay.GetComponent<Text>().text = "" + LapTimer.millisecondCount;
            }
            PlayerPrefs.SetInt("minuteSave", LapTimer.minuteCount);
            PlayerPrefs.SetInt("secondSave", LapTimer.secondCount);
            PlayerPrefs.SetFloat("millisecondSave", LapTimer.millisecondCount);
            PlayerPrefs.SetFloat("gameTime", LapTimer.gameTime);

        }
            LapTimer.minuteCount = 0;
            LapTimer.secondCount = 0;
            LapTimer.millisecondCount = 0;
            LapTimer.gameTime = 0;
            lapCounter.GetComponent<Text>().text = "" + totalLaps;
            halfLapTrigger.SetActive(true);
            lapCompleteTrigger.SetActive(false);
        
    }
}
