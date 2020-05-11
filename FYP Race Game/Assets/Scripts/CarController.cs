using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(Rigidbody))]

public class CarController : MonoBehaviour
{
    GameObject[] Cubes;
    GameObject[] AICars;
    GameObject[] PlayerCars;
    public InputManager IM;
    public UIManager UIM;
    public List<WheelCollider> throttleWheels;
    public List<GameObject> steerWheels;
    public List<GameObject> meshes;
    public float breakHorsePower;
    public float maxTurn;
    public Transform centerMass;
    public Rigidbody RB;
    public float brakes;
    public int positionNumber;
    
    public float currentCubeStartTime = 0f;
    public float rubberBandSpeedLimit = 100f;
    public int lapActive = 1;// variable for calculation when car is after final cube but not yet crossed the line and to make sure car completes all points before completing lap
    public int currentCube = 0;
    public int currentLap = 0;
    public float currentLapStartTime;
    public float[] lapTimes;
    public float topSpeed;
    

    void Start()
    {
        IM = GetComponent<InputManager>();
        RB = GetComponent<Rigidbody>();
        if (centerMass)
        {
            RB.centerOfMass = centerMass.localPosition;
        }

        Cubes = GameObject.FindGameObjectsWithTag("Cubes"); //Puts all cubes into and array, allows cars to follow the waypoints.
        AICars = GameObject.FindGameObjectsWithTag("AICars"); //Puts all AI cars into and array, allows cars calculate rank in race.
        PlayerCars = GameObject.FindGameObjectsWithTag("MyCar"); //Puts all Player cars into and array, allows cars calculate rank in race.
        
        currentLapStartTime = LapTimer.gameTime;
        lapTimes = new float[3]; //Declare array for laptimes, adding 1 to allow for lap 0

    }




    //This counts the number of laps
    private void OnTriggerEnter(Collider other)
    {
        float bestLapTime;

        if (other.gameObject.name == "LapCompleteTrigger" && lapActive == 1)
        {
            //log laptime
            lapTimes[currentLap] = LapTimer.gameTime - currentLapStartTime;
            currentLapStartTime = LapTimer.gameTime;
            currentLap++;
            lapActive = 0;
            
            // check main lap counter
            if (currentLap > LapComplete.totalLaps)
            {
                LapComplete.totalLaps = currentLap;
            }
            //Reset Display Timer
            LapTimer.minuteCount = 0;
            LapTimer.secondCount = 0;
            LapTimer.millisecondCount = 0;

            //Update Best Times
            if(currentLap >= 2)
            {
                bestLapTime = lapTimes[1];
                for (int i = 2; i < lapTimes.Length && i < currentLap; i++)
                {
                    if (lapTimes[i] < bestLapTime)
                    {
                        bestLapTime = lapTimes[i];
                    }
                }


                LapTimer.bestLapMinutes = "" + Mathf.FloorToInt(bestLapTime / 60) + ":";
                if (Mathf.FloorToInt(bestLapTime - Mathf.FloorToInt(bestLapTime / 60) * 60 ) < 10)
                {
                    LapTimer.bestLapSeconds = "0" + Mathf.FloorToInt(bestLapTime - Mathf.FloorToInt(bestLapTime / 60) * 60) + ".";
                }
                else 
                { 
                    LapTimer.bestLapSeconds = "" + Mathf.FloorToInt(bestLapTime - Mathf.FloorToInt(bestLapTime / 60) * 60) + "."; 
                }
                LapTimer.bestLapMilliseconds = "" + (bestLapTime - Mathf.FloorToInt(bestLapTime)) * 10 + "";

            }

        }

    }

    void Update()
    {
        // speedometer
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
                wheel.brakeTorque = brakes * Time.fixedDeltaTime / 30f;
            }
            else
            {
                if (transform.InverseTransformVector(RB.velocity).z * 2.23694f  > topSpeed * (rubberBandSpeedLimit / 100))
                {
                    wheel.motorTorque = 0f;
                }
                else
                {
                    wheel.motorTorque = breakHorsePower * Time.fixedDeltaTime * IM.throttle;
                }
                
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
                || otherCar.GetComponent<CarEngine>().currentLap * Cubes.Length + otherCar.GetComponent<CarEngine>().currentCube > averagePlayerCube + 2))
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
                || otherCar.GetComponent<CarController>().currentLap * Cubes.Length + otherCar.GetComponent<CarController>().currentCube > averagePlayerCube + 2))
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

    private void checkWayPointDistance()
    {
        int myCube;
        int otherCube;
        int carsAhead = 0;

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
            //move to next cube, but if at end of lap wrap around to cube 0
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
                        if (otherCar.name != this.name)
                        {
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
