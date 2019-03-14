using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class ARController : MonoBehaviour
{
    [Tooltip("A prefab for tracking and visualizing detected planes.")]
    public GameObject DetectedPlanePrefab;

    public GameObject trackingCanvas;
    
    // A list to hold all planes ARCore is tracking in the current frame. This object is used across
    // the application to avoid per-frame allocations.        
    List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();
    
    // True if the app is in the process of quitting due to an ARCore connection error, otherwise false.        
    bool m_IsQuitting = false;

    [Space]
    public StateMachine stateMachine;

    [HideInInspector]
    public bool tracking = false;

    private void Start()
    {
        trackingCanvas.SetActive(true);
    }

    void Update()
    {
        _UpdateApplicationLifecycle();

        // Hide snackbar when currently tracking at least one plane.
        Session.GetTrackables(m_AllPlanes);

        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
            {
                // Tracking has been (re)acquired: 
                if (tracking == false)
                {
                    tracking = true;
                    trackingCanvas.SetActive(false);
                }                
                return;
            }
        }

        // Tracking has been lost: 
        if (tracking)
        {
            tracking = false;
            trackingCanvas.SetActive(true);
            stateMachine.ToSetupState();
        }        
    }
    
    // Check and update the application lifecycle.        
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            SendAndroidMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            SendAndroidMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    private void _DoQuit()
    {
        Application.Quit();
    }

    public void SendAndroidMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
