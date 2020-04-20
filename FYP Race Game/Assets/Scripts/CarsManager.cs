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

    void Start()
    {
        playerOne.GetComponent<CarController>().enabled = true;
        carbonCar.GetComponent<CarEngine>().enabled = true;
        grayCar.GetComponent<CarEngine>().enabled = true;
        redCar.GetComponent<CarEngine>().enabled = true;
        yellowCar.GetComponent<CarEngine>().enabled = true;

    }
}
