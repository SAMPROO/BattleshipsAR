using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using Lean.Touch;

public class SetupState : MonoBehaviour, IGameState
{
    public GameObject areaRoot;

    [Header("Canvas")]
    public GameObject setupCanvas;
    public Toggle reCenter, scale, rotate;
    public Button readyButton;

    [Header("ARcore")]
    public ARController arCrtl;
    public GameObject planeGenerator, pointCloud;

    [Header("Scale")]
    public float minScale = 0.1f;
    public float maxScale = 20f;
    public float startScale = 10f;

    StateMachine state;
    Transform cameraRoot;
    TrackableHitFlags raycastFilter;

    void Start()
    {
        state = GetComponent<StateMachine>();
        cameraRoot = Camera.main.transform.parent;

        // Raycast against the location the player touched to search for planes.
        raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

        SetScale(startScale);
    }

    public void EnterState()
    {
        planeGenerator.SetActive(true);
        pointCloud.SetActive(true);

        setupCanvas.SetActive(false);

        reCenter.isOn = true;
        scale.isOn = false;
        rotate.isOn = false;

        reCenter.interactable = false;
        scale.interactable = false;
        rotate.interactable = false;
        readyButton.interactable = false;

        areaRoot.SetActive(false);
    }

    public void ExecuteState()
    {
        if (arCrtl.tracking == false)
            return;

        setupCanvas.SetActive(true);

        if (reCenter.isOn)
        {
            ReCenter();
        }
        else if (scale.isOn)
        {
            Scale();
        }
        else if (rotate.isOn)
        {
            Rotate();
        }

        //ReCenter();
        //Rotate();
        //Scale();

        if (readyButton.GetComponent<HoldDownButton>().Ready())
        {
            state.ToPreviousState();
        }
    }

    public void ExitState()
    {
        planeGenerator.SetActive(false);
        pointCloud.SetActive(false);

        setupCanvas.SetActive(false);
    }

    void Scale()
    {
        // Get the fingers we want to use
        var fingers = LeanSelectable.GetFingers(true, false, 2);

        // Calculate pinch scale, and make sure it's valid
        var pinchScale = LeanGesture.GetPinchScale(fingers);

        if (pinchScale != 1.0f)
        {
            var newScale = cameraRoot.localScale.x * (2f - pinchScale);

            SetScale(newScale);
        }
    }

    void SetScale(float newScale)
    {
        newScale = Mathf.Clamp(newScale, minScale, maxScale);

        ScaleAround(cameraRoot, Vector3.zero, newScale * Vector3.one);
    }

    void ScaleAround(Transform target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.localScale.x; // relative scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.localScale = newScale;
        target.localPosition = FP;
    }

    Vector2 touchBegin;
    float rotationAngle;

    void Rotate()
    {
        //// Get the fingers we want to use
        //var fingers = LeanSelectable.GetFingers(true, false, 2);

        //var twistDegrees = LeanGesture.GetTwistDegrees(fingers);

        //if (twistDegrees != 0.0f)
        //{
        //    cameraRoot.RotateAround(Vector3.zero, Vector3.up, twistDegrees);
        //}

        if (Input.touchCount == 1 && state.IsPointerOverUIObject() == false)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchBegin = touch.position;
                rotationAngle = 0f;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 pivot = Camera.main.WorldToScreenPoint(Vector3.zero);
                Vector2 touchMoved = touch.position;

                float angle = Vector2.SignedAngle(touchBegin - pivot, touchMoved - pivot);

                float deltaAngle = angle - rotationAngle;

                cameraRoot.RotateAround(Vector3.zero, Vector3.up, deltaAngle);

                rotationAngle = angle;
            }
        }
    }

    void ReCenter()
    {
        if ((Input.touchCount == 1 || (Input.GetTouch(0)).phase == TouchPhase.Began) && state.IsPointerOverUIObject() == false)
        {
            var screenPosition = Input.GetTouch(0).position;
            if (Frame.Raycast(screenPosition.x, screenPosition.y, raycastFilter, out TrackableHit hit))
            {
                var position = hit.Pose.position;

                // account for difference in scale
                position.Scale(cameraRoot.localScale);

                // account for difference in rotation
                position = cameraRoot.rotation * position;

                cameraRoot.position = -position;

                if (reCenter.interactable == false)
                {
                    reCenter.interactable = true;
                    scale.interactable = true;
                    rotate.interactable = true;
                    readyButton.interactable = true;

                    areaRoot.SetActive(true);
                }
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(30, 90, 200, 100), "Tracking: " + arCrtl.tracking.ToString(), state.myStyle);
    }
}
