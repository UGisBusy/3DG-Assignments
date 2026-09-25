using System;
using System.Collections.Generic;
using UnityEngine;

public class State : IState
{
    List<ILink> links;
    bool isTransitionEventBound;

    Action enter, exit;

    public State(Action enter = null, Action exit = null)
    {
        links = new List<ILink>();
        isTransitionEventBound = false;
        this.enter = enter;
        this.exit = exit;
    }

    public virtual void Enter()
    {
        EnableAllLinks();
        enter?.Invoke();
    }

    public virtual void Exit()
    {
        exit?.Invoke();
        DisableAllLinks();
    }

    public void SetEnter(Action enter)
    {
        this.enter = enter;
    }

    public void SetExit(Action exit)
    {
        this.exit = exit;
    }

    public void BindTransitionEvent(Action<IState> onTransition)
    {
        if (isTransitionEventBound || onTransition == null)
        {
            return;
        }

        isTransitionEventBound = true;
        foreach (ILink link in links)
        {
            link.SetOutputListener(onTransition);
        }
    }

    public virtual void AddLink(ILink link)
    {
        if (!links.Contains(link))
        {
            links.Add(link);
            isTransitionEventBound = false;
        }
    }

    public virtual void RemoveLink(ILink link)
    {
        if (links.Contains(link))
        {
            link.Disable();
            links.Remove(link);
        }
    }

    public virtual void RemoveAllLinks()
    {
        DisableAllLinks();
        links.Clear();
    }

    public virtual void EnableAllLinks()
    {
        foreach (ILink link in links)
        {
            link.Enable();
        }
    }

    public virtual void DisableAllLinks()
    {
        foreach (ILink link in links)
        {
            link.Disable();
        }
    }
}
