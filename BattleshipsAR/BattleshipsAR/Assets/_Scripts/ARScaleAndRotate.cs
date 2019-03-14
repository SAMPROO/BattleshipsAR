using UnityEngine;
using UnityEngine.AI;
using Lean.Touch;
using GoogleARCore;


public class ARScaleAndRotate : MonoBehaviour
{
    [Tooltip("Ignore fingers with StartedOverGui?")]
    public bool IgnoreStartedOverGui = true;

    [Tooltip("Ignore fingers with IsOverGui?")]
    public bool IgnoreIsOverGui;

    [Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
    public int RequiredFingerCount;

    [Range(-1.0f, 1.0f)]
    public float WheelSensitivity;

    [Space]
    public float startScale = 1;
    public Transform playArea;
    float pinchScale;

    void Update()
    {
        // Get the fingers we want to use
        var fingers = LeanSelectable.GetFingers(true, false, 2);

        // Calculate pinch scale, and make sure it's valid
        pinchScale = LeanGesture.GetPinchScale(fingers);

        if (pinchScale != 1.0f)
        {
            //// Perform the scaling
            //Scale(transform.localScale * pinchScale);

            transform.localScale = transform.localScale * (2f - pinchScale);

            ////Store the position for scaling.
            //Vector3 position = Vector3.zero;

            ////Scale the position by the scale of the root transform
            //position.Scale(transform.localScale);

            ////Position the root transform by negative the hitPosition
            //transform.localPosition = position * -1;

        }
    }

    //private void OnGUI()
    //{
    //    GUIStyle myStyle = new GUIStyle();
    //    myStyle.fontSize = 10;
    //    GUI.Label(new Rect(30, 90, 200, 100), "Camera scale: " + transform.lossyScale.ToString(), myStyle);
    //    GUI.Label(new Rect(30, 120, 200, 100), "Camera pos: " + Camera.main.transform.position.ToString(), myStyle);
    //    GUI.Label(new Rect(30, 150, 200, 100), "pinchScale: " + (2f - pinchScale), myStyle);
        
    //    //GUILayout.Label(currentState.ToString());
    //}
}