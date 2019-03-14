using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PlayAreaScale : MonoBehaviour
{
    // Start is called before the first frame update
    public float playAreaScale;
    public float originScale;
    public PlayerMovement pm;

    NavMeshSurface navMesh;
    public NavMeshAgent player;

    float scale;


    void Start()
    {
        pm=GetComponentInChildren<PlayerMovement>();
        originScale = pm.moveRange;

        scale = transform.localScale.x;

        navMesh = GetComponent<NavMeshSurface>();
        navMesh.BuildNavMesh();
    }

    public void OnSlide(float _input)
    {
        //transform.localScale = Vector3.one * ((playAreaScale +_input)/10f);
        //pm.moveRange = originScale * (transform.localScale.x * 10f);

        player.enabled = false;

        transform.localScale = Vector3.one * (scale + _input);
        pm.moveRange = originScale * transform.localScale.x * .2f;

        navMesh.BuildNavMesh();

        player.enabled = true;
    }
}
