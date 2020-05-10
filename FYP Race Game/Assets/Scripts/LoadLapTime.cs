using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadLapTime : MonoBehaviour
{
    public int minuteCount;
    public int secondCount;
    public float millisecondCount;
    public GameObject minuteDisplay;
    public GameObject secondDisplay;
    public GameObject millisecondDisplay;
   
    // Start is called before the first frame update
    void Start()
    {
        minuteCount = PlayerPrefs.GetInt("minuteSave");
        secondCount = PlayerPrefs.GetInt("secondSave");
        millisecondCount = PlayerPrefs.GetFloat("millisecondSave");

        minuteDisplay.GetComponent<Text>().text = "" + minuteCount;
        secondDisplay.GetComponent<Text>().text = "" + secondCount;
        millisecondDisplay.GetComponent<Text>().text = "" + millisecondCount;

    }

}
