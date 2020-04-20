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

    //public GameObject lapTimeBox;

    void OnTriggerEnter(Collider other)
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

        LapTimer.minuteCount = 0;
        LapTimer.secondCount = 0;
        LapTimer.millisecondCount = 0;

        halfLapTrigger.SetActive(true);
        lapCompleteTrigger.SetActive(false);
    }
}
