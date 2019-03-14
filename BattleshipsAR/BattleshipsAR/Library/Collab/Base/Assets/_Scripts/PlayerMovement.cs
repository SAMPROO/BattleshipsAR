using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public Button confirmMoveBtn;
    Vector3 pos;
    Quaternion rot;
    public float moveRange;

    public GameObject marker;
    public GameObject rangeMarker;

    NavMeshAgent agent;

    public StateMachine stateMachine;

    void Start()
    {
        var scale = Vector3.one * moveRange / transform.lossyScale.x;
        scale.y = 1;
        rangeMarker.transform.localScale = scale;

        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject(0) == false) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("PlayArea"))
                {
                    
                    Vector3 newMove = (hit.point - pos);

                    if (newMove.magnitude>moveRange)
                    {
                        marker.transform.position = pos + newMove.normalized * moveRange;
                    }
                    else
                    {
                        marker.transform.position = hit.point;
                    }
                    
                    confirmMoveBtn.interactable = true;

                    agent.enabled = false;
                    transform.position = pos;
                    transform.rotation = rot;
                    agent.enabled = true;

                    agent.SetDestination(marker.transform.position);
                }
            }            
        }        
    }

    public void WarpToDestination()
    {
        //agent.Warp(agent.destination);
        if (agent.pathPending 
            && agent.remainingDistance > agent.stoppingDistance 
            && (agent.hasPath || agent.velocity.sqrMagnitude != 0f))
        { 
            Vector3[] path = agent.path.corners;
            Vector3 pointA = path[path.Length - 2];
            Vector3 pointB = path[path.Length - 1];
            Vector3 AtoB = pointB - pointA;

            agent.enabled = false;
            transform.rotation = Quaternion.LookRotation(AtoB, Vector3.up);
            transform.position = pointB;
            agent.enabled = true;
        }

        
    }

    //private void FixedUpdate()
    //{
    //    if (moving)
    //    {
    //        agent.SetDestination(marker.transform.position);

    //        // Check if we've reached the destination
    //        if (!agent.pathPending)
    //        {
    //            if (agent.remainingDistance <= agent.stoppingDistance)
    //            {
    //                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
    //                {
    //                    // Done
    //                    Debug.Log("VALMISTA TULI");
    //                    waitingInput = true;
    //                    moving = false;
    //                }
    //            }
    //        }

    //        //if (Vector3.Distance(transform.position, marker.transform.position) > 0.001f)
    //        //{
    //        //    transform.position = Vector3.Lerp(transform.position, marker.transform.position, 0.1f);
    //        //}
    //        //else
    //        //{
    //        //    Debug.Log("VALMISTA TULI");
    //        //    moveDone = true;
    //        //    waitingInput = true;
    //        //    moveConfirmed = false;
    //        //    moving = false;
    //        //}

    //    }
    //}
}

