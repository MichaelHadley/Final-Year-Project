using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(Rigidbody))]

public class CarController : MonoBehaviour
{
    public InputManager IM;
    public UIManager UIM;
    public List<WheelCollider> throttleWheels;
    public List<GameObject> steerWheels;
    public List<GameObject> meshes;
    public float strengthCoefficient = 10000f;
    public float maxTurn = 20f;
    public Transform centerMass;
    public Rigidbody RB;
    public float brakes;
    // Start is called before the first frame update
    void Start()
    {
        IM = GetComponent<InputManager>();
        RB = GetComponent<Rigidbody>();
        
        if (centerMass)
        {
            RB.centerOfMass = centerMass.localPosition;
        }
    }

    void Update()
    {
        UIM.changeText(transform.InverseTransformVector(RB.velocity).z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(WheelCollider wheel in throttleWheels)
        {
            if(IM.brake)
            {
                wheel.motorTorque = 0f;
                wheel.brakeTorque = brakes * Time.fixedDeltaTime;
            }
            else
            {
                wheel.motorTorque = strengthCoefficient * Time.fixedDeltaTime * IM.throttle;
                wheel.brakeTorque = 0f;
            }
        }

        foreach(GameObject wheel in steerWheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle = maxTurn * IM.steer;
            wheel.transform.localEulerAngles = new Vector3(0f, IM.steer * maxTurn, 0f);
        }

        foreach(GameObject mesh in meshes)
        {
            // Wheels rotate forwards when driving forward and can rotate backwards when reversing
            mesh.transform.Rotate(RB.velocity.magnitude * (transform.InverseTransformDirection(RB.velocity).z >= 0 ? 1 : -1) / (2 * Mathf.PI * 0.35f), 0f, 0f);
        }
    }
}
