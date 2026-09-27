using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputLink : ILink
{
    Action<IState> outputListener;
    InputAction inputAction;
    IState nextState;

    public InputLink(IState nextState, InputAction inputAction)
    {
        this.nextState = nextState;
        this.inputAction = inputAction;
        outputListener = null;

        this.inputAction.Enable();
    }

    public void Enable()
    {
        inputAction.performed += OnSourceEventRaised;
    }

    public void Disable()
    {
        inputAction.performed -= OnSourceEventRaised;
    }

    public void SetOutputListener(Action<IState> outputListener)
    {
        this.outputListener = outputListener;
    }

    private void OnSourceEventRaised(InputAction.CallbackContext context)
    {
        if (outputListener == null)
        {
            Debug.LogWarning($"{nameof(Link)} has no outputListener");
        }

        outputListener?.Invoke(nextState);
    }
}
