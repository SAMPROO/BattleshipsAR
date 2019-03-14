using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 moveLoc;
    public GameObject marker;
    bool waitingInput = true,
        moveDone = false,
        moveConfirmed = false,
        moving = false;

    string waitingInputString = "";
    string moveDoneString = "";
    string moveConfirmedString = "";
    string movingString = "";

    void Start()
    {
        moveLoc = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && waitingInput)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("PlayArea"))
                {
                    moveLoc = hit.point;
                    marker.transform.position = moveLoc;
                }
            }
            
        }
        if (moveDone == false && moveConfirmed)
        {
            if(Vector3.Distance(transform.position,moveLoc)>=0.05f)
            {
                transform.position = Vector3.Lerp(transform.position, moveLoc, 0.1f);
            }
            else
            {
                Debug.Log("VALMISTA TULI");
                moveDone = true;
                waitingInput = true;
                moveConfirmed = false;
            }
            
        }
    }

    public void ConfirmMove()
    {
        moveConfirmed = true;
        waitingInput = false;
    }

    void OnGUI()
    {

        waitingInputString = GUI.TextField(new Rect(250, 93, 250, 25), "waitingInput: " + waitingInput.ToString(), 40);
        moveDoneString = GUI.TextField(new Rect(250, 125, 250, 25), "moveDone: " + moveDone.ToString(), 40);
        moveConfirmedString = GUI.TextField(new Rect(250, 157, 250, 25), "moveConfirmed: " + moveConfirmed.ToString(), 40);
        movingString = GUI.TextField(new Rect(300, 189, 200, 25), "moving: " + moving.ToString(), 40);
    }
}

