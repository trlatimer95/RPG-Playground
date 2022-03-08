using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour, IRaycastable
{
    public string displayName;

    public string DisplayName()
    {
        return displayName;
    }

    public bool HandleRaycast(PlayerController callingController)
    {
        Debug.Log("Add wood to inventory");
        Destroy(gameObject);
        return true;
    }
}
