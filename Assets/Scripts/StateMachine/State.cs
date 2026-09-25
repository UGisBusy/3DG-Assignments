using System;
using System.Collections.Generic;
using UnityEngine;

public class State : IState
{
    List<ILink> links;
    bool isTransitionEventBound;

    public State()
    {
        links = new List<ILink>();
        isTransitionEventBound = false;
    }

    public virtual void Enter()
    {
        EnableAllLinks();
    }

    public virtual void Exit()
    {
        DisableAllLinks();
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
