using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IRaycastable
{
    public string displayName;
    public float cooldownTime = 4f;

    private Animator animator;
    private bool cooldown;
    private float cooldownElapsed = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (cooldown)
        {
            cooldownElapsed += Time.deltaTime;
            if (cooldownElapsed > cooldownTime)
            {
                cooldown = false;
                cooldownElapsed = 0;
            }
        }
    }

    string IRaycastable.DisplayName()
    {
        return displayName;
    }

    bool IRaycastable.HandleRaycast(PlayerController callingController)
    {
        if (!cooldown)
        {
            cooldown = true;
            animator.SetBool("open", !animator.GetBool("open"));
        }
        return true;
    }
}
