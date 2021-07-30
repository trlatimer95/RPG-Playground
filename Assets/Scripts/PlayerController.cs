using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Transform groundCheck;
    public Camera mainCamera;
    public float speed = 12f;
    public float groundDistance = 0.4f;
    public float mouseSensitivity = 100f;
    public float jumpHeight = 3f;
    public int maxInteractableDistance = 10;
    public LayerMask groundMask;
    //public Text interactableText;

    Vector3 velocity;
    bool isGrounded;
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandlePlayerMovement();
    }

    private void HandlePlayerMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
        }

        velocity += Physics.gravity * 2 * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    //private void HandlePlayerInteraction()
    //{
    //    RaycastHit hit;
    //    Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * maxInteractableDistance, Color.white);
    //    if (Physics.Raycast(mainCamera.transform.position + new Vector3(0, 0, 0.2f), mainCamera.transform.TransformDirection(Vector3.forward), out hit, maxInteractableDistance) && hit.transform.gameObject.GetComponent<IRaycastable>() != null)
    //    {
    //        interactableText.text = hit.transform.gameObject.GetComponent<IRaycastable>().DisplayName();
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            hit.transform.gameObject.GetComponentInParent<IRaycastable>().HandleRaycast(this);
    //        }
    //    }
    //    else
    //    {
    //        interactableText.text = "";
    //    }
    //}

}
