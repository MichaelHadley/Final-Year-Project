using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    GameObject[] Cubes;
    GameObject[] AICars;
    GameObject[] PlayerCars;
    public Transform WayPoints;
    public float maxSteerAngle;
    public WheelCollider wheelFrontLeft;
    public WheelCollider wheelFrontRight;
    public WheelCollider wheelRearLeft;
    public WheelCollider wheelRearRight;
    public float maxMotorTorque;
    public float currentSpeed;
    public float topSpeed;
    public Vector3 centerOfMass;
    public float maxBreakTorque;
    public List <float> targetSpeedList;
    public int positionNumber;

    public int currentCube = 0;
    public int currentLap = 0;
    public float currentLapStartTime;
    public float[] lapTimes;

    public int lapActive = 1;// variable for calculation when car is after final cube but not yet crossed the line and to make sure car completes all points before completing lap
    private float avoidFront = 0f;
    private float avoidLeft = 0f;
    private float avoidRight = 0f;
    public float currentCubeStartTime = 0f;
    public float rubberBandSpeedLimit = 100f;

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Rigidbody>().centerOfMass = centerOfMass; //Edit center of mass in the inspector

        Cubes = GameObject.FindGameObjectsWithTag("Cubes"); //Puts all cubes into and array, allows cars to follow the waypoints.
        AICars = GameObject.FindGameObjectsWithTag("AICars"); //Puts all AI cars into and array, allows cars calculate rank in race.
        PlayerCars = GameObject.FindGameObjectsWithTag("MyCar"); //Puts all Player cars into and array, allows cars calculate rank in race.

        currentLapStartTime = LapTimer.gameTime;
        lapTimes = new float[3]; //Declare array for laptimes, adding 1 to allow for lap 0
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "LapCompleteTrigger" && lapActive == 1)
        {
            //log laptime
            lapTimes[currentLap] = LapTimer.gameTime - currentLapStartTime;
            currentLapStartTime = LapTimer.gameTime;
            
            currentLap ++;
            lapActive = 0;
            
                // check main lap counter
            if (currentLap > LapComplete.totalLaps)
            {
                LapComplete.totalLaps = currentLap;
            }
        }
        
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        RadarCheck();
        applySteer();
        Drive();
        checkWayPointDistance();
        rubberBanding();
    }

    private void rubberBanding()
    {
        float timeInCube;
        int maxAICube = 0;
        int runningAICars = 0;
        int totalRunningAICubes = 0;
        int runningPlayerCars = 0;
        int totalRunningPlayerCubes = 0;
        foreach (GameObject otherCar in AICars)
        {
            timeInCube = LapTimer.gameTime - otherCar.GetComponent<CarEngine>().currentCubeStartTime;
            if (timeInCube < 10)
            {
                runningAICars++;
                totalRunningAICubes += otherCar.GetComponent<CarEngine>().currentLap * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube;
                if (otherCar.GetComponent<CarEngine>().currentLap * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube > maxAICube)
                {
                    maxAICube = otherCar.GetComponent<CarEngine>().currentLap * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube;
                }
            }
        }
        foreach (GameObject otherCar in PlayerCars)
        {
            timeInCube = LapTimer.gameTime - otherCar.GetComponent<CarController>().currentCubeStartTime;
            //if (timeInCube < 10) No check for stopped player cars

            runningPlayerCars++;
            totalRunningPlayerCubes += otherCar.GetComponent<CarController>().currentLap * Cubes.Length + otherCar.GetComponent<CarController>().currentCube;

        }

        int averageAICube;

        if (runningAICars == 0)
        {
            averageAICube = 9999;
        }
        else
        {
            averageAICube = totalRunningAICubes / runningAICars;
        }

        int averagePlayerCube;
        if (runningPlayerCars == 0)
        {
            averagePlayerCube = 9999;
        }
        else
        {
            averagePlayerCube = totalRunningPlayerCubes / runningPlayerCars;
        }

        foreach (GameObject otherCar in AICars)
        {
            //If more than 5 cubes ahead of average positions slow AI car
            if (otherCar.GetComponent<CarEngine>().currentLap * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube > averageAICube + 5
                || otherCar.GetComponent<CarEngine>().currentLap * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube > averagePlayerCube + 5)
            {
                otherCar.GetComponent<CarEngine>().rubberBandSpeedLimit = 30f;
            }
            //If more than 2 - 5 cubes ahead of average positions and already slowed, then do not release until 2 cubes or less
            else if (otherCar.GetComponent<CarEngine>().rubberBandSpeedLimit < 100f &&
                (otherCar.GetComponent<CarEngine>().currentLap * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube > averageAICube + 2
                || otherCar.GetComponent<CarEngine>().currentLap * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube > averagePlayerCube + 2) )
            {
                otherCar.GetComponent<CarEngine>().rubberBandSpeedLimit = otherCar.GetComponent<CarEngine>().rubberBandSpeedLimit;
            }
            //Release restriction if have not got up to 5 ahead or was more than 5 ahead and is now within 2
            else
            {
                otherCar.GetComponent<CarEngine>().rubberBandSpeedLimit = 100f;
            }
        }
        foreach (GameObject otherCar in PlayerCars)
        {
            //If more than 5 cubes ahead of front AI car  or average of all players (for adding multiplayer later) then slow player car
            if (otherCar.GetComponent<CarController>().currentLap * Cubes.Length + otherCar.GetComponent<CarController>().currentCube > maxAICube + 5
                || otherCar.GetComponent<CarController>().currentLap * Cubes.Length + otherCar.GetComponent<CarController>().currentCube > averagePlayerCube + 5)
            {
                otherCar.GetComponent<CarController>().rubberBandSpeedLimit = 30f;
            }
            //If more than 2 - 5 cubes ahead of front AI car and already slowed, then do not release until 2 cubes or less
            else if (otherCar.GetComponent<CarController>().rubberBandSpeedLimit < 100f &&
                (otherCar.GetComponent<CarController>().currentLap * Cubes.Length + otherCar.GetComponent<CarController>().currentCube > maxAICube + 2
                || otherCar.GetComponent<CarController>().currentLap * Cubes.Length + otherCar.GetComponent<CarController>().currentCube > averagePlayerCube + 2) )
            {
                otherCar.GetComponent<CarController>().rubberBandSpeedLimit = otherCar.GetComponent<CarController>().rubberBandSpeedLimit;
            }
            //Release restriction if have not got up to 5 ahead or was more than 5 ahead and is now within 2
            else
            {
                otherCar.GetComponent<CarController>().rubberBandSpeedLimit = 100f;
            }
        }


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
        if (currentSpeed < topSpeed && currentSpeed < Cubes[currentCube].GetComponent<Cubes>().targetSpeed && currentSpeed < topSpeed * (rubberBandSpeedLimit /100))
        {
            //limit acceleration when at slower speeds
            if(currentSpeed < 40f)
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
                wheelFrontLeft.brakeTorque = maxBreakTorque / 100;
                wheelFrontRight.brakeTorque = maxBreakTorque / 100;
                wheelRearLeft.brakeTorque = maxBreakTorque / 100;
                wheelRearRight.brakeTorque = maxBreakTorque / 100;
            }
            else if (currentSpeed > 30f)
            {
                wheelFrontLeft.brakeTorque = maxBreakTorque / 200;
                wheelFrontRight.brakeTorque = maxBreakTorque / 200;
                wheelRearLeft.brakeTorque = maxBreakTorque / 200;
                wheelRearRight.brakeTorque = maxBreakTorque / 200;
            }
            else
            {
                wheelFrontLeft.brakeTorque = maxBreakTorque / 400;
                wheelFrontRight.brakeTorque = maxBreakTorque / 400;
                wheelRearLeft.brakeTorque = maxBreakTorque / 400;
                wheelRearRight.brakeTorque = maxBreakTorque / 400;
            }
            
        }
        
    }




    private void checkWayPointDistance()
    {
        int myCube;
        int otherCube;
        int carsAhead=0;

        //if car is within 10 of waypoint then move to next one. Compares centre to centre only
        if (Vector3.Distance(transform.position, Cubes[currentCube].transform.position) < 10f)
        {
            //myCube and other Cube is cube number we are up to, including lap * Cubes.Length to allow for the same cube on multiple laps
            // if just before start of the lap, then lap active is 1 and calulates as if on next lap
            myCube = (currentLap + lapActive) * Cubes.Length + currentCube;

            //Check all the AI cars and player cars to count how many are ahead (to workout position of this car)
            //Needs to be done in 2 loops as there are 2 different classes used
            foreach (GameObject otherCar in AICars)
            {
                if (otherCar.name != this.name)
                {
                    // if just before start of the lap, then lap active is 1 and calulates as if on next lap
                    otherCube = (otherCar.GetComponent<CarEngine>().currentLap + otherCar.GetComponent<CarEngine>().lapActive) * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube;
                    if (otherCube > myCube)
                    {
                        carsAhead++;
                    }

                }
            }
            foreach (GameObject otherCar in PlayerCars)
            {
                if (otherCar.name != this.name)
                {
                    // if just before start of the lap, then lap active is 1 and calulates as if on next lap
                    otherCube = (otherCar.GetComponent<CarController>().currentLap + otherCar.GetComponent<CarController>().lapActive) * Cubes.Length + otherCar.GetComponent<CarController>().currentCube;
                    if (otherCube > myCube)
                    {
                        carsAhead++;
                    }
                }
            }

            //Update the position of the cars, looping twice for the different classes
            foreach (GameObject otherCar in AICars)
            {
                if (otherCar.name != this.name)
                {
                    //If other car is not one that is ahead and not already behind this car then it has been overtaken and the position moves back by one (eg 3rd goes to 4th)
                    if (otherCar.GetComponent<CarEngine>().positionNumber > carsAhead && otherCar.GetComponent<CarEngine>().positionNumber <= positionNumber)
                    {
                        otherCar.GetComponent<CarEngine>().positionNumber += 1;
                        if (otherCar.GetComponent<CarEngine>().positionNumber == 1)
                        {
                            LapTimer.pos1Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                        }
                        else if (otherCar.GetComponent<CarEngine>().positionNumber == 2)
                        {
                            LapTimer.pos2Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                        }
                        else if (otherCar.GetComponent<CarEngine>().positionNumber == 3)
                        {
                            LapTimer.pos3Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                        }
                        else if (otherCar.GetComponent<CarEngine>().positionNumber == 4)
                        {
                            LapTimer.pos4Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                        }
                        else if (otherCar.GetComponent<CarEngine>().positionNumber == 5)
                        {
                            LapTimer.pos5Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                        }
                    }

                }
            }
            foreach (GameObject otherCar in PlayerCars)
            {
                if (otherCar.name != this.name)
                {
                    if (otherCar.GetComponent<CarController>().positionNumber > carsAhead && otherCar.GetComponent<CarController>().positionNumber <= positionNumber)
                    {
                        otherCar.GetComponent<CarController>().positionNumber += 1;
                        if (otherCar.GetComponent<CarController>().positionNumber == 1)
                        {
                            LapTimer.pos1Name = otherCar.GetComponent<CarController>().name.Substring(16);
                        }
                        else if (otherCar.GetComponent<CarController>().positionNumber == 2)
                        {
                            LapTimer.pos2Name = otherCar.GetComponent<CarController>().name.Substring(16);
                        }
                        else if (otherCar.GetComponent<CarController>().positionNumber == 3)
                        {
                            LapTimer.pos3Name = otherCar.GetComponent<CarController>().name.Substring(16);
                        }
                        else if (otherCar.GetComponent<CarController>().positionNumber == 4)
                        {
                            LapTimer.pos4Name = otherCar.GetComponent<CarController>().name.Substring(16);
                        }
                        else if (otherCar.GetComponent<CarController>().positionNumber == 5)
                        {
                            LapTimer.pos5Name = otherCar.GetComponent<CarController>().name.Substring(16);
                        }
                    }
                }
            }
            //Update this car position to be 1 after cars ahead (2 cars ahead would mean that position is 3rd)
            positionNumber = carsAhead + 1;
            if (positionNumber == 1)
            {
                LapTimer.pos1Name = name.Substring(16);
            }
            else if (positionNumber == 2)
            {
                LapTimer.pos2Name = name.Substring(16);
            }
            else if (positionNumber == 3)
            {
                LapTimer.pos3Name = name.Substring(16);
            }
            else if (positionNumber == 4)
            {
                LapTimer.pos4Name = name.Substring(16);
            }
            else if (positionNumber == 5)
            {
                LapTimer.pos5Name = name.Substring(16);
            }




            //move to next cube, but if at end of lap wrap around to cube 0
            currentCubeStartTime = LapTimer.gameTime;
            if (currentCube == Cubes.Length - 1)
            {
                currentCube = 0;
                lapActive = 1;
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
                    //myCube and other Cube is cube number we are up to, including lap * Cubes.Length to allow for the same cube on multiple laps
                    // if just before start of the lap, then lap active is 1 and calulates as if on next lap
                    myCube = (currentLap + lapActive) * Cubes.Length + currentCube;
                    
                    //Check all the AI cars and player cars to count how many are ahead (to workout position of this car)
                    //Needs to be done in 2 loops as there are 2 different classes used
                    foreach (GameObject otherCar in AICars)
                    {
                        if(otherCar.name != this.name)
                        {
                            // if just before start of the lap, then lap active is 1 and calulates as if on next lap
                            otherCube = (otherCar.GetComponent<CarEngine>().currentLap + otherCar.GetComponent<CarEngine>().lapActive) * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube;
                            if (otherCube > myCube)
                            {
                                carsAhead++;
                            }
                            
                        }
                    }
                    foreach (GameObject otherCar in PlayerCars)
                    {
                        if (otherCar.name != this.name)
                        {
                            // if just before start of the lap, then lap active is 1 and calulates as if on next lap
                            otherCube = (otherCar.GetComponent<CarController>().currentLap + otherCar.GetComponent<CarController>().lapActive) * Cubes.Length + otherCar.GetComponent<CarController>().currentCube;
                            if (otherCube > myCube)
                            {
                                carsAhead++;
                            }
                        }
                    }

                    //Update the position of the cars, looping twice for the different classes
                    foreach (GameObject otherCar in AICars)
                    {
                        if (otherCar.name != this.name)
                        {   
                            //If other car is not one that is ahead and not already behind this car then it has been overtaken and the position moves back by one (eg 3rd goes to 4th)
                            if (otherCar.GetComponent<CarEngine>().positionNumber > carsAhead && otherCar.GetComponent<CarEngine>().positionNumber <= positionNumber)
                            {
                                otherCar.GetComponent<CarEngine>().positionNumber += 1;
                                if (otherCar.GetComponent<CarEngine>().positionNumber == 1)
                                {
                                    LapTimer.pos1Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                                }
                                else if (otherCar.GetComponent<CarEngine>().positionNumber == 2)
                                {
                                    LapTimer.pos2Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                                }
                                else if (otherCar.GetComponent<CarEngine>().positionNumber == 3)
                                {
                                    LapTimer.pos3Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                                }
                                else if (otherCar.GetComponent<CarEngine>().positionNumber == 4)
                                {
                                    LapTimer.pos4Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                                }
                                else if (otherCar.GetComponent<CarEngine>().positionNumber == 5)
                                {
                                    LapTimer.pos5Name = otherCar.GetComponent<CarEngine>().name.Substring(16);
                                }
                            }

                        }
                    }
                    foreach (GameObject otherCar in PlayerCars)
                    {
                        if (otherCar.name != this.name)
                        {
                            if (otherCar.GetComponent<CarController>().positionNumber > carsAhead && otherCar.GetComponent<CarController>().positionNumber <= positionNumber)
                            {
                                otherCar.GetComponent<CarController>().positionNumber += 1;
                                if (otherCar.GetComponent<CarController>().positionNumber == 1)
                                {
                                    LapTimer.pos1Name = otherCar.GetComponent<CarController>().name.Substring(16);
                                }
                                else if (otherCar.GetComponent<CarController>().positionNumber == 2)
                                {
                                    LapTimer.pos2Name = otherCar.GetComponent<CarController>().name.Substring(16);
                                }
                                else if (otherCar.GetComponent<CarController>().positionNumber == 3)
                                {
                                    LapTimer.pos3Name = otherCar.GetComponent<CarController>().name.Substring(16);
                                }
                                else if (otherCar.GetComponent<CarController>().positionNumber == 4)
                                {
                                    LapTimer.pos4Name = otherCar.GetComponent<CarController>().name.Substring(16);
                                }
                                else if (otherCar.GetComponent<CarController>().positionNumber == 5)
                                {
                                    LapTimer.pos5Name = otherCar.GetComponent<CarController>().name.Substring(16);
                                }
                            }
                        }
                    }
                    //Update this car position to be 1 after cars ahead (2 cars ahead would mean that position is 3rd)
                    positionNumber = carsAhead + 1;
                    if (positionNumber == 1)
                    {
                        LapTimer.pos1Name = name.Substring(16);
                    }
                    else if (positionNumber == 2)
                    {
                        LapTimer.pos2Name = name.Substring(16);
                    }
                    else if (positionNumber == 3)
                    {
                        LapTimer.pos3Name = name.Substring(16);
                    }
                    else if (positionNumber == 4)
                    {
                        LapTimer.pos4Name = name.Substring(16);
                    }
                    else if (positionNumber == 5)
                    {
                        LapTimer.pos5Name = name.Substring(16);
                    }

                    currentCubeStartTime = LapTimer.gameTime;
                    if (currentCube == Cubes.Length - 1)
                    {
                        currentCube = 0;
                        lapActive = 1;
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
