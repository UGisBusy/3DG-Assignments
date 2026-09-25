using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IState CurrentState { get; private set; }

    public StateMachine()
    {
        CurrentState = null;
    }

    public void EnterState(IState state)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        if (CurrentState != null)
        {
            ExitState();
        }

        CurrentState = state;
        CurrentState.BindTransitionEvent(OnStateTransition);
        CurrentState.Enter();
    }

    public void ExitState()
    {
        if (CurrentState == null)
        {
            return;
        }

        CurrentState.Exit();
        CurrentState = null;
    }

    private void OnStateTransition(IState nextState)
    {
        ExitState();
        EnterState(nextState);
    }
}
