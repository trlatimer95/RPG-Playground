using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IRaycastable
{
    public GameObject gate;
    public string displayName;

    private bool isOpen = false;

    string IRaycastable.DisplayName()
    {
        return displayName;
    }

    bool IRaycastable.HandleRaycast(PlayerController callingController)
    {
        if (isOpen)
        {
            gate.transform.Translate(0, -3, 0);
            isOpen = false;
        } else
        {
            gate.transform.Translate(0, 3, 0);
            isOpen = true;
        }
        return true;
    }
}
