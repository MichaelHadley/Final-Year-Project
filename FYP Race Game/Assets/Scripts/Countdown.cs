using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Countdown : MonoBehaviour
{
    public GameObject countDown;
    public GameObject lapTimer;
    public GameObject carsManager;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(countStart());
    }

    IEnumerator countStart()
    {
        yield return new WaitForSeconds(0.5f);
        countDown.GetComponent<Text>().text = "3";
        countDown.SetActive(true);
        yield return new WaitForSeconds(1);
        countDown.SetActive(false);
        countDown.GetComponent<Text>().text = "2";
        countDown.SetActive(true);
        yield return new WaitForSeconds(1);
        countDown.SetActive(false);
        countDown.GetComponent<Text>().text = "1";
        countDown.SetActive(true);
        yield return new WaitForSeconds(1);
        countDown.SetActive(false);
        lapTimer.SetActive(true);
        carsManager.SetActive(true);
    }
}
