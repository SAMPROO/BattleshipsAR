using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour
{
    public Transform xAxis, yAxis;
    public int resolution = 10;

    LineRenderer arc;
    float velocity, gravity, radianAngle;

    void Start()
    {
        arc = GetComponent<LineRenderer>();
        gravity = -Mathf.Abs(Physics.gravity.y);
        velocity = transform.parent.GetComponent<TurretControllerV3>().velocity;
    }

    public void RenderArc(Vector3 direction, float distance)
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * -direction.x;

        float flightTime = (2f * velocity * Mathf.Sin(radianAngle)) / gravity;

        if (flightTime < 0)
        {
            flightTime = -flightTime;
        }

        for (int i = 0; i <= resolution; i++)
        {
            float x = distance * i / (float)resolution;
            float y = x * (Mathf.Tan(radianAngle) - -gravity * x / (2 * Mathf.Pow(velocity * Mathf.Cos(radianAngle), 2) ) );
            arcArray[i] = Quaternion.AngleAxis(direction.y, Vector3.up) * new Vector3(0, y, x) + xAxis.position;
        }

        arc.positionCount = resolution + 1;
        arc.SetPositions(arcArray);
    }
}
