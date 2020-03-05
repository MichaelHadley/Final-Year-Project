using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour{

    public Color lineColor;

    private List<Transform> Cube = new List<Transform>();

    private void OnDrawGizmos(){
    Gizmos.color = lineColor;

        Transform[] WayPointsTransform = GetComponentsInChildren<Transform>();
        Cube = new List<Transform>();

        for(int i = 0; i < WayPointsTransform.Length; i++)
        {
            if(WayPointsTransform[i] != transform)
            {
                Cube.Add(WayPointsTransform[i]);
            }
        }

        for (int i = 0; i < Cube.Count; i++)
        {
            Vector3 CurrentCube = Cube[i].position;
            Vector3 PreviousCube = Vector3.zero;

            if (i > 0)
            {
                PreviousCube = Cube[i - 1].position;
            } else if (i == 0 && Cube.Count > 1)
            {
                PreviousCube = Cube[Cube.Count - 1].position;
            }

            Gizmos.DrawLine(PreviousCube, CurrentCube);
        }
    }

}
