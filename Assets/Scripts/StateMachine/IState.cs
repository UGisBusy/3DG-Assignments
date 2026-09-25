using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    void Exit();
    void SetEnter(Action enter);
    void SetExit(Action exit);
    void BindTransitionEvent(Action<IState> onTransition);
    void AddLink(ILink link);
    void RemoveLink(ILink link);
    void RemoveAllLinks();
    void EnableAllLinks();
    void DisableAllLinks();
}
