using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{

    public Transform WayPoints;
    public float maxSteerAngle = 40f;
    public WheelCollider wheelFrontLeft;
    public WheelCollider wheelFrontRight;
    public WheelCollider wheelRearLeft;
    public WheelCollider wheelRearRight;
    public float maxMotorTorque = 250f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;
    public float maxBreakTorque = 150f;
    public bool isBraking;

    [Header("Sensors")]
    public float sensorLength = 2f;
    public float frontSensorPosition = 0.5f;

    private List<Transform> Cube;
    private int currentCube = 0;

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        Transform[] WayPointsTransform = WayPoints.GetComponentsInChildren<Transform>();
        Cube = new List<Transform>();

        for (int i = 0; i < WayPointsTransform.Length; i++)
        {
            if (WayPointsTransform[i] != WayPoints.transform)
            {
                Cube.Add(WayPointsTransform[i]);
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Sensors();
        applySteer();
        Drive();
        checkWayPointDistance();
        braking();
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 SensorStartPosition = transform.position;
        SensorStartPosition.z += frontSensorPosition;

        if(Physics.Raycast(SensorStartPosition, transform.forward, out hit, sensorLength))
        {

        }
        Debug.DrawLine(SensorStartPosition, hit.point);
    }

    private void applySteer()
    {
        Vector3 reletiveVector = transform.InverseTransformPoint(Cube[currentCube].position);
        reletiveVector = reletiveVector / reletiveVector.magnitude; // devide reletiveVector by length of reletiveVector 
        float newSteer = (reletiveVector.x / reletiveVector.magnitude) * maxSteerAngle; // reletiveVector.magnitude equals the length of the vector
        wheelFrontLeft.steerAngle = newSteer;
        wheelFrontRight.steerAngle = newSteer;
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFrontLeft.radius * wheelFrontLeft.rpm * 60 / 1000; //calculates current speed based off how fast the wheel is spinning

        if (currentSpeed < maxSpeed && !isBraking)
        {
            wheelFrontLeft.motorTorque = maxMotorTorque;
            wheelFrontRight.motorTorque = maxMotorTorque;
        }
        else
        {
            wheelFrontLeft.motorTorque = 0;
            wheelFrontRight.motorTorque = 0;
        }
    }

    private void checkWayPointDistance()
    {
        if (Vector3.Distance(transform.position, Cube[currentCube].position) < 10f)
        {
            if (currentCube == Cube.Count - 1)
            {
                currentCube = 0;
            }
            else
            {
                currentCube++;
            }

        }
    }

    private void braking()
    {
        if(isBraking) {
            wheelRearLeft.brakeTorque = maxBreakTorque;
            wheelRearRight.brakeTorque = maxBreakTorque;
        }
        else
        {
            wheelRearLeft.brakeTorque = 0;
            wheelRearRight.brakeTorque = 0;
        }
    }
}
