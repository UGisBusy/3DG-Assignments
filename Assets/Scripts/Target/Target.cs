using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    public bool IsBeingAttack { get; private set; }

    public void BeTargeted()
    {
        IsBeingAttack = true;

        // TODO: for test 
        gameObject.SetActive(false);
    }

    private void Start()
    {
        IsBeingAttack = false;
    }
}
