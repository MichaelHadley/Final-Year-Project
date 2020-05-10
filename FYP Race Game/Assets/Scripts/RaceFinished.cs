using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceFinished : MonoBehaviour
{
    public GameObject playerOne;
    public GameObject carbonCar;
    public GameObject grayCar;
    public GameObject redCar;
    public GameObject yellowCar;
    public GameObject loadTimer;
    public GameObject lapTimer;
    public GameObject gameOverCam;
    public GameObject lapCompleteTrigger;
    void OnTriggerEnter(Collider other)
    {
        this.GetComponent<BoxCollider>().enabled = false;
        playerOne.SetActive(false);
        carbonCar.SetActive(false);
        grayCar.SetActive(false);
        redCar.SetActive(false);
        yellowCar.SetActive(false);
        lapCompleteTrigger.SetActive(false);
        carbonCar.GetComponent<CarEngine>().enabled = false;
        grayCar.GetComponent<CarEngine>().enabled = false;
        redCar.GetComponent<CarEngine>().enabled = false;
        yellowCar.GetComponent<CarEngine>().enabled = false;
        loadTimer.SetActive(false);
        lapTimer.SetActive(false);
        playerOne.GetComponent<CarController>().enabled = false;
        playerOne.SetActive(true);
        carbonCar.SetActive(true);
        grayCar.SetActive(true);
        redCar.SetActive(true);
        yellowCar.SetActive(true);
        gameOverCam.SetActive(true);
    }
}
