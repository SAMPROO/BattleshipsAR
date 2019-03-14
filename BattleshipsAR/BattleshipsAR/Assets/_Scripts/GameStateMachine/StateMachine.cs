using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class StateMachine : MonoBehaviour
{
    [Header("Debug")]
    public int fontSize;

    [HideInInspector]
    public IGameState currentState;
    private IGameState previousState;

    [HideInInspector]
    public GUIStyle myStyle;

    [System.Serializable]
    public class ShipPrefabs
    {
        public GameObject shipPrefab;
        public int cost;
    }

    [Header("ShipPrefabs")]
    public List<ShipPrefabs> shipPrefabs = new List<ShipPrefabs>();

    [System.Serializable]
    public class Player
    {
        public List<GameObject> playerShips;
        public Color playerColor;
        public int money = 100;
        public int actionPoints = 100;
    }

    [Header("Players")]
    public List<Player> players = new List<Player>();

    public Transform canvasHolder;

    private void Awake()
    {
        foreach (GameObject canvas in canvasHolder)
        {
            canvas.SetActive(false);
        }
    }

    private void Start()
    {

        myStyle = new GUIStyle {fontSize = fontSize};

        // start from setup state
        ToSetupState();

        // after initial setupState is moveState
        previousState = GetComponent<ShipPlacementState>();

    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.ExecuteState();
        }
    }

    public void ChangeState(IGameState newState)
    {
        if (currentState != newState)
        {
            if (currentState != null)
            {
                currentState.ExitState();
            }
            previousState = currentState;

            currentState = newState;
        }

        currentState.EnterState();

        SendAndroidMessage(currentState.ToString());
    }

    public void ToPreviousState()
    {
        ChangeState(previousState);
    }

    public void ToSetupState()
    {
        ChangeState(GetComponent<SetupState>());
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(30, 30, 200, 100), currentState != null ? currentState.ToString() : "StateMachine (null)", myStyle);
        GUI.Label(new Rect(30, 60, 200, 100), previousState.ToString(), myStyle);
    }

    public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
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

