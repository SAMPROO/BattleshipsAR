using UnityEngine;
using UnityEngine.UI;

public class PinchZoom : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.0001f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

    public Text text;

    public float speed;
    public float minSize = 0f;
    public float maxSize = 0.5f;

    void Update()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            deltaMagnitudeDiff = Mathf.Clamp(deltaMagnitudeDiff * speed, minSize, maxSize);

            text.text = deltaMagnitudeDiff.ToString();

            transform.localScale += Vector3.one * deltaMagnitudeDiff;

            //// Store both touches.
            //Touch touchZero = Input.GetTouch(0);
            //Touch touchOne = Input.GetTouch(1);

            //// Find the position in the previous frame of each touch.
            //Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            //Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            //// Find the magnitude of the vector (the distance) between the touches in each frame.
            //float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            //float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            //// Find the difference in the distances between each frame.
            //float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            //// If the camera is orthographic...
            //if (firstPersonCamera.orthographic)
            //{
            //    // ... change the orthographic size based on the change in distance between the touches.
            //    firstPersonCamera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

            //    // Make sure the orthographic size never drops below zero.
            //    firstPersonCamera.orthographicSize = Mathf.Max(firstPersonCamera.orthographicSize, 0.1f);
            //}
            //else
            //{
            //    // Otherwise change the field of view based on the change in distance between the touches.
            //    firstPersonCamera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

            //    // Clamp the field of view to make sure it's between 0 and 180.
            //    firstPersonCamera.fieldOfView = Mathf.Clamp(firstPersonCamera.fieldOfView, 0.1f, 179.9f);
            //}

            //playAreaScale.OnSlide(deltaMagnitudeDiff * perspectiveZoomSpeed);
        }
    }
}