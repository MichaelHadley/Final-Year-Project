using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LapComplete : MonoBehaviour
{
    public GameObject lapCompleteTrigger;
//    public GameObject halfLapTrigger;

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
       lapCounter.GetComponent<Text>().text = "" + totalLaps;
    }
}
