using UnityEngine;
using UnityEngine.UI;

public class AimState : MonoBehaviour, IGameState
{
    private StateMachine state;

    public GameObject aimCanvas;
    public Button readyButton;

    public TurretControllerV3[] turrets;

    public Transform targetMarker;

    RaycastHit hittt;

    private void Start()
    {
        state = GetComponent<StateMachine>();
    }

    public void EnterState()
    {
        aimCanvas.SetActive(true);
        targetMarker.gameObject.SetActive(true);
    }

    public void ExecuteState()
    {
        if (Input.GetMouseButtonDown(0) && state.IsPointerOverUIObject() == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("PlayArea"))
                {
                    targetMarker.position = hit.point;

                    hittt = hit;
                    foreach (var turret in turrets)
                        turret.TakeAim(hit.point);
                }
            }
        }

        if (readyButton.GetComponent<HoldDownButton>().Ready())
        {
            //state.ChangeState(GetComponent<ResolveState>());
            Shoot();
        }
    }

    public void Shoot()
    {
        for (int i = 0; i < turrets.Length; i++)
        {
            turrets[i].FireProjectile();
        }
    }


    public void ExitState()
    {
        aimCanvas.SetActive(false);
        targetMarker.gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(30, 70, 100, 100), "Aim hit point: " + hittt.point.ToString(), state.myStyle);
    }
    

    //private StateController state;

    //bool playersReady;

    //public AimState(StateController stateController)
    //{
    //    state = stateController;
    //}

    //public void UpdateState()
    //{
    //    PlayerAim();
    //}

    //public void ToAimState()
    //{
    //}

    //public void ToMoveState()
    //{
    //}

    //public void ToResolveState()
    //{
    //    state.currentState = state.resolveState;
    //}

    //void PlayerAim()
    //{
    //    if (playersReady)
    //    {
    //        ToResolveState();
    //    }
    //}

    //public void ToSetupState()
    //{
    //    throw new System.NotImplementedException();
    //}
}
