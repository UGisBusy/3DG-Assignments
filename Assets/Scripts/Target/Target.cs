using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    public bool IsBeingAttack { get; private set; }

    public void BeTargeted()
    {
        IsBeingAttack = true;
    }

    public void BeAttacked()
    {
        // deactivate, later get destory by GameplayManager
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        IsBeingAttack = false;
    }
}
