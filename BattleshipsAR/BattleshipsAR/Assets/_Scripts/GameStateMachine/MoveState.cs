using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveState : MonoBehaviour, IGameState
{
    public PlayerMovement playerMovement;

    [Header("Range")]
    public GameObject targetMarker;
    public GameObject rangeIndicator;

    [Header("Canvas")]
    public GameObject moveCanvas;
    public Button readyButton;

    private StateMachine state;

    private void Start()
    {
        state = GetComponent<StateMachine>();
    }

    public void EnterState()
    {
        playerMovement.enabled = true;

        moveCanvas.SetActive(true);
        readyButton.interactable = false;

        rangeIndicator.SetActive(true);
        targetMarker.SetActive(false);
    }

    public void ExecuteState()
    {
        if (Input.GetMouseButtonDown(0) && state.IsPointerOverUIObject() == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("PlayArea"))
                {
                    playerMovement.MoveTo(hit.point);

                    readyButton.interactable = true;
                    targetMarker.SetActive(true);
                }
            }
        }

        if (readyButton.GetComponent<HoldDownButton>().Ready())
        {
            SendAndroidMessage("MOVE READY");
            playerMovement.CompleteMove();
            state.ChangeState(GetComponent<AimState>());
        }
    }

    public void ExitState()
    {
        playerMovement.enabled = false;
        moveCanvas.SetActive(false);
        rangeIndicator.SetActive(false);
        targetMarker.SetActive(false);
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
