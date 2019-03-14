using UnityEngine;
using UnityEngine.UI;

public class ShipPlacementState : MonoBehaviour, IGameState
{
    private StateMachine state;

    public GameObject[] placementAreas;

    public Transform shipHolder;

    [Header("Canvas")]
    public GameObject placementCanvas;
    public Button readyButton;
    public Toggle shipToggle;

    int index;
    bool movingShip;
    GameObject selectedShip; //For moving a ship

    LayerMask placementLayer = 1 << 10;

    public void EnterState()
    {
        state = GetComponent<StateMachine>();

        for (int i = 0; i < placementAreas.Length; i++)
        {
            placementAreas[i].SetActive(false);
        }

        placementCanvas.SetActive(true);
        readyButton.interactable = false;
    }

    public void ExecuteState()
    {
        if (index > 0)
        {
            placementAreas[index - 1].SetActive(false);
        }

        if (shipToggle.isOn)
        {
            PlaceShip();
        }
        else
        {
            MoveShip();
        }

        if (readyButton.GetComponent<HoldDownButton>().Ready())
        {
            index++;

            if (index == state.players.Count)
            {
                state.ChangeState(GetComponent<MoveState>());
            }

            readyButton.interactable = false;
        }
    }

    public void ExitState()
    {
        for (int i = 0; i < placementAreas.Length; i++)
        {
            placementAreas[i].SetActive(false);
        }

        placementCanvas.SetActive(false);

    }

    public void PlaceShip()
    {
        placementAreas[index].SetActive(true);

        if (Input.GetMouseButtonDown(0) && state.IsPointerOverUIObject() == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayer))
            {
                if (index < state.players.Count && state.players[index].money >= state.shipPrefabs[0].cost)
                {
                    GameObject ship =
                        Instantiate(state.shipPrefabs[0].shipPrefab, hit.point, placementAreas[index].transform.rotation, shipHolder);

                    state.players[index].playerShips.Add(ship);
                    state.players[index].money -= state.shipPrefabs[0].cost;

                    readyButton.interactable = true;
                }
            }
        }
    }

    public void MoveShip()
    {
        if (Input.touchCount == 1 && state.IsPointerOverUIObject() == false)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider.CompareTag("Ship"))
                    {
                        foreach (GameObject ship in state.players[index].playerShips)
                        {
                            if (ship == hit.collider.gameObject)
                            {
                                movingShip = true;
                                selectedShip = ship;
                                break;
                            }
                        }
                    }
                }

            }
            else if (touch.phase == TouchPhase.Moved && movingShip)
            {
                selectedShip.transform.position = Camera.main.ScreenToWorldPoint(touch.position);
            }
        }
        else
        {
            movingShip = false;
        }
    }
}
