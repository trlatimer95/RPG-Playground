using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IRaycastable
{
    public string displayName;
    public GameObject logPrefab;

    private bool cutDown = false;

    public string DisplayName()
    {
        return displayName;
    }

    public bool HandleRaycast(PlayerController callingController)
    {
        if (!cutDown)
        {
            Rigidbody treeRigidBody = transform.gameObject.GetComponent<Rigidbody>();
            treeRigidBody.isKinematic = false;
            treeRigidBody.AddForceAtPosition(treeRigidBody.rotation * Vector3.forward * 20f, new Vector3(0, callingController.transform.position.y, 0));

            cutDown = true;
        }
        else
        {
            Debug.Log("Chopped tree");
            if (logPrefab != null)
            {
                Instantiate(logPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        return true;
    }
}
