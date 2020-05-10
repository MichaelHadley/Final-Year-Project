using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarsManager : MonoBehaviour
{
    public GameObject playerOne;
    public GameObject carbonCar;
    public GameObject grayCar;
    public GameObject redCar;
    public GameObject yellowCar;
    public GameObject loadTimer;
    public GameObject lapTimer;
    void Start()
    {
        playerOne.GetComponent<CarController>().enabled = true;
        carbonCar.GetComponent<CarEngine>().enabled = true;
        grayCar.GetComponent<CarEngine>().enabled = true;
        redCar.GetComponent<CarEngine>().enabled = true;
        yellowCar.GetComponent<CarEngine>().enabled = true;
        loadTimer.GetComponent<LoadLapTime>().enabled = true;
        lapTimer.GetComponent<LapTimer>().enabled = true;
    }
}
