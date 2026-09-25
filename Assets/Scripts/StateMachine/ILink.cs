using System;
using UnityEngine;

public interface ILink
{
    void Enable();
    void Disable();
    void SetOutputListener(Action<IState> outputListener);
}
