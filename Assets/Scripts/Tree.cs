using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IRaycastable
{
    public string displayName;
    public GameObject stumpPrefab;

    private bool cutDown = false;

    public string DisplayName()
    {
        return displayName;
    }

    public bool HandleRaycast(PlayerController callingController)
    {
        if (!cutDown)
        {
            transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            Instantiate(stumpPrefab, transform.position, Quaternion.identity);
            cutDown = true;
        } else
        {
            Debug.Log("Chopped tree");
            Destroy(gameObject);
        }
        return true;
    }

}
