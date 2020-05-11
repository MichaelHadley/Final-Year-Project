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
    public LapComplete lapComplete;

    void Start()
    {
        //Once the countdown class has finished counting turn on all the games classes below
        lapTimer.GetComponent<LapTimer>().enabled = true;
        playerOne.GetComponent<CarController>().enabled = true;
        carbonCar.GetComponent<CarEngine>().enabled = true;
        grayCar.GetComponent<CarEngine>().enabled = true;
        redCar.GetComponent<CarEngine>().enabled = true;
        yellowCar.GetComponent<CarEngine>().enabled = true;
        
    }
}
