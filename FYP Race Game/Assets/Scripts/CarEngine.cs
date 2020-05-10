using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    GameObject[] Cubes;
    public Transform WayPoints;
    public float maxSteerAngle = 40f;
    public WheelCollider wheelFrontLeft;
    public WheelCollider wheelFrontRight;
    public WheelCollider wheelRearLeft;
    public WheelCollider wheelRearRight;
    public float maxMotorTorque;
    public float currentSpeed;
    public float topSpeed = 150f;
    public Vector3 centerOfMass;
    public float maxBreakTorque = 100f;
    public List <float> targetSpeedList;


    private int currentCube = 0;
    private float avoidFront = 0f;
    private float avoidLeft = 0f;
    private float avoidRight = 0f;

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Rigidbody>().centerOfMass = centerOfMass; //Edit center of mass in the inspector

        Cubes = GameObject.FindGameObjectsWithTag("Cubes"); //Puts all cubes into and array, allows cars to follow the waypoints.

       
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        RadarCheck();
        applySteer();
        Drive();
        checkWayPointDistance();
    }

    private void RadarCheck()
    {
        //Avoid any cars in front, drive to left or right of them
        //9 is the layer number assigned to cars, detection will then only detect collisions with cars
        int layerMask = 1 << 9;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 1, transform.forward, out hit, 5, layerMask))
        {
            Debug.DrawRay(transform.position, transform.forward * 6, Color.red);
            //check if car in front is more left or right and go around
            if (transform.InverseTransformPoint(hit.transform.position).x < 0)
            {
                avoidFront = maxSteerAngle;
            }
            else
            {
                avoidFront = -maxSteerAngle;
            }

        }
        else
        {
            //if hit nothing set avoid front to 0.
            Debug.DrawRay(transform.position, transform.forward * 6, Color.green);
            avoidFront = 0f;
        }


        //Avoid anything at the side of the car, based on the distance from the front wheels
        //Left Wheel
        Vector3 leftWheelPos = wheelFrontLeft.gameObject.transform.position;
        Vector3 leftOutDirection = new Vector3(-transform.forward.z, 0, transform.forward.x);
        if (Physics.Raycast(leftWheelPos, leftOutDirection, 1))
        {
            Debug.DrawRay(leftWheelPos, leftOutDirection * 1, Color.red);
            avoidLeft = 0.25f * maxSteerAngle; //Turn part of max steering, not as harsh as avoiding front end collision
        }
        else
        {
            Debug.DrawRay(leftWheelPos, leftOutDirection * 1, Color.green);
            avoidLeft = 0f;
        }


        //Right Wheel
        Vector3 rightWheelPos = wheelFrontRight.gameObject.transform.position;
        Vector3 rightOutDirection = new Vector3(transform.forward.z, 0, -transform.forward.x);
        if (Physics.Raycast(rightWheelPos, rightOutDirection, 1))
        {
            Debug.DrawRay(rightWheelPos, rightOutDirection * 1, Color.red);
            avoidRight = -0.25f * maxSteerAngle; //Turn part of max steering, not as harsh as avoiding front end collision
        }
        else
        {
            Debug.DrawRay(rightWheelPos, rightOutDirection * 1, Color.green);
            avoidRight = 0f;
        }

    }


    private void applySteer()
    {
        // Work out direction to waypoint
        Vector3 reletiveVector = transform.InverseTransformPoint(Cubes[currentCube].transform.position);
        // Normalize the vector to get a length one vector and apply x component to steering
        float newSteer = reletiveVector.normalized.x * maxSteerAngle;


        //Add collision avoidance steering to new steer to get the total steering direction
        float totalSteer = newSteer + avoidLeft + avoidRight + avoidFront;
        //check that total steer has not gone above max steer limit and cap it at that if it has (negative version needed if steering is to the left)
        if(totalSteer > maxSteerAngle)
        {
            totalSteer = maxSteerAngle;
        }
        else if(totalSteer < -maxSteerAngle)
        {
            totalSteer = -maxSteerAngle;
        }


        //apply steering to front wheels
        wheelFrontLeft.steerAngle = totalSteer;
        wheelFrontRight.steerAngle = totalSteer;
    }
    
    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFrontLeft.radius * wheelRearLeft.rpm * 60 / 1000; //calculates current speed based off how fast the wheel is spinning. Uses rear wheels to avoid false values from wheel spin

        //limit speed to topspeed or target speed of current waypoint        
        if (currentSpeed < topSpeed && currentSpeed < Cubes[currentCube].GetComponent<Cubes>().targetSpeed)
        {
            //limit acceleration when at slower speeds
            if (currentSpeed < 20f)
            {
                wheelFrontLeft.motorTorque = maxMotorTorque / 4;
                wheelFrontRight.motorTorque = maxMotorTorque / 4;
            }
            else if(currentSpeed < 40f)
            {
                wheelFrontLeft.motorTorque = maxMotorTorque / 2;
                wheelFrontRight.motorTorque = maxMotorTorque / 2;
            }
            else
            {
                wheelFrontLeft.motorTorque = maxMotorTorque;
                wheelFrontRight.motorTorque = maxMotorTorque;
            }

            wheelRearLeft.motorTorque = 0;
            wheelRearRight.motorTorque = 0;

            //don't brake when accellerating
            wheelFrontLeft.brakeTorque = 0f;
            wheelFrontRight.brakeTorque = 0f;
            wheelRearLeft.brakeTorque = 0f;
            wheelRearRight.brakeTorque = 0f;
        }
        else
        {
            //if gooing too fast, stop accellerating and slow down
            wheelFrontLeft.motorTorque = 0;
            wheelFrontRight.motorTorque = 0;
            wheelRearLeft.motorTorque = 0;
            wheelRearRight.motorTorque = 0;

            //braking applied to all 4 wheels and depends on speed.
            if (currentSpeed > 80f)
            {
                wheelFrontLeft.brakeTorque = 4f;
                wheelFrontRight.brakeTorque = 4f;
                wheelRearLeft.brakeTorque = 4f;
                wheelRearRight.brakeTorque = 4f;
            }
            else if (currentSpeed > 30f)
            {
                wheelFrontLeft.brakeTorque = 2f;
                wheelFrontRight.brakeTorque = 2f;
                wheelRearLeft.brakeTorque = 2f;
                wheelRearRight.brakeTorque = 2f;
            }
            else
            {
                wheelFrontLeft.brakeTorque = 1f;
                wheelFrontRight.brakeTorque = 1f;
                wheelRearLeft.brakeTorque = 1f;
                wheelRearRight.brakeTorque = 1f;
            }
            
        }
        
    }

    private void checkWayPointDistance()
    {
        //if car is within 10 of waypoint then move to next one. Compares centre to centre only
        if (Vector3.Distance(transform.position, Cubes[currentCube].transform.position) < 10f)
        {
            //move to next cube, but if at end of lap wrap around to cube 0
            if (currentCube == Cubes.Length - 1)
            {
                currentCube = 0;
            }
            else
            {
                currentCube++;
            }
        }
        else
        {
            //If miss the centre of the waypoint (eg going off on a corner), then check if close to the corners of the cube (cubes made very wide)

            //Create mapping matrix to map mesh back to real world (vertices of mesh are realive to 0,0,0 not the position of the cube on the map)
            Matrix4x4 localToWorld = Cubes[currentCube].transform.localToWorldMatrix;
            
            //loop though each vertex to see if in range.
            foreach (Vector3 vertex in Cubes[currentCube].transform.GetComponent<MeshFilter>().mesh.vertices)
            {
                //convert vertex to postion in world which can then be compared to the car's position
                Vector3 vertexInWorld = localToWorld.MultiplyPoint3x4(vertex);
                if (Vector3.Distance(transform.position, vertexInWorld) < 10f)
                {
                    if (currentCube == Cubes.Length - 1)
                    {
                        currentCube = 0;
                        break;//Break used so if match is found the code exits the foreach loop and doesn't increase current cube by more than 1
                    }
                    else
                    {
                        currentCube++;
                        break;//Break used so if match is found the code exits the foreach loop and doesn't increase current cube by more than 1
                    }
                }
            }
        }
    }
}
