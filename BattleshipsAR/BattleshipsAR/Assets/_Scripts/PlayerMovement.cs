using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public float moveRange;

    public Transform targetMarker;
    public Transform rangeIndicator;

    NavMeshAgent agent;

    void Start()
    {
        var scale = moveRange / transform.lossyScale.x * Vector3.one;
        scale.y = 1;
        rangeIndicator.localScale = scale;

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        Vector3 newMove = targetPosition - rangeIndicator.position;

        if (newMove.magnitude > moveRange)
        {
            targetMarker.position = rangeIndicator.position + newMove.normalized * moveRange;
        }
        else
        {
            targetMarker.position = targetPosition;
        }

        transform.rotation = rangeIndicator.rotation;
        transform.position = rangeIndicator.position;

        agent.enabled = true;

        agent.SetDestination(targetMarker.position);
    }

    public void CompleteMove()
    {
        agent.enabled = false;

        Vector3[] path = agent.path.corners;
        if (path.Length >= 2)
        {
            Vector3 pointA = path[path.Length - 2];
            Vector3 pointB = path[path.Length - 1];
            Vector3 AtoB = pointB - pointA;

            transform.rotation = Quaternion.LookRotation(AtoB, Vector3.up);
            transform.position = pointB;
        }        

        rangeIndicator.rotation = transform.rotation;
        rangeIndicator.position = transform.position;
    }
}

