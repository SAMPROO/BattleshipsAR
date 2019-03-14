using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveState : MonoBehaviour, IGameState
{
    private StateMachine state;


    private void Start()
    {
        state = GetComponent<StateMachine>();
    }

    public void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public void ExecuteState()
    {
        throw new System.NotImplementedException();
    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public void OnLoseBoard()
    {
        throw new System.NotImplementedException();
    }

    public void OnFindBoard()
    {
        throw new System.NotImplementedException();
    }
    //private StateController state;

    //public ResolveState(StateController stateController)
    //{
    //    state = stateController;
    //}

    //public void UpdateState()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void ToAimState()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void ToMoveState()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void ToResolveState()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void ToSetupState()
    //{
    //    throw new System.NotImplementedException();
    //}
}
