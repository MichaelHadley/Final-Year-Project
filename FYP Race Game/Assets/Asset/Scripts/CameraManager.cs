using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject focus;
    public float distance = 5f;
    public float height = 2f;
    public float dampening = 1f;

    private int camMode = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            camMode = (camMode + 1) % 2; // cycle through the modulus to use 2 cameras, it can't go passed 2 so it will cycle through 0 and 1
        }
        switch(camMode){
            case 1:
                transform.position = focus.transform.position + focus.transform.TransformDirection(new Vector3(0f, height, -distance));
                transform.LookAt(focus.transform);
                break;
            default:
            transform.position = Vector3.Lerp(transform.position, focus.transform.position + focus.transform.TransformDirection(new Vector3(0f, height, -distance)), dampening * Time.deltaTime);
            transform.LookAt(focus.transform);
                break;
        }

        Debug.Log(camMode);
    }
}
