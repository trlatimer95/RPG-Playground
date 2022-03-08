using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float playerSpeed = 2.0f;
    public float jumpHeight = 3f;
    public float gravityMultiplier = 3.0f;
    public float groundDistance = 0.04f;
    public LayerMask groundLayer;

    [Header("Look")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("Interaction")]
    public int maxInteractableDistance = 10;
    public Text interactableText;
    private GameObject currentInteractableObject;

    [Header("Components")]
    public CharacterController controller;
    public Camera mainCamera;
    public Transform groundChecker;

    [Header("Status")]
    public Vector3 playerVelocity;
    public bool isGrounded;

    // Input values
    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (controller == null)
            controller = gameObject.GetComponent<CharacterController>();
        if (mainCamera == null)
            mainCamera = gameObject.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        HandlePlayerMovement();
        CheckForInteractable();
    }

    private void HandlePlayerMovement()
    {
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundLayer, QueryTriggerInteraction.Ignore);
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = 0f;

        // Move Player
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Look around
        if (lookInput != Vector2.zero)
        {
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation,-90, 90);
           
            mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.Rotate(Vector3.up * mouseX);
        }

        playerVelocity.y += gravityMultiplier * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void CheckForInteractable()
    {
        RaycastHit hit;
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * maxInteractableDistance, Color.white);
        if (Physics.Raycast(mainCamera.transform.position + new Vector3(0, 0, 0.2f), mainCamera.transform.TransformDirection(Vector3.forward), out hit, maxInteractableDistance) && hit.transform.gameObject.GetComponent<IRaycastable>() != null)
        {
            interactableText.text = "(E) " + hit.transform.gameObject.GetComponent<IRaycastable>().DisplayName();
            currentInteractableObject = hit.transform.gameObject;
        }
        else
        {
            interactableText.text = "";
            currentInteractableObject = null;
        }
    }

    private void TryInteract()
    {
        if (currentInteractableObject != null)
        {
            Debug.Log("Interacted");
            currentInteractableObject.GetComponentInParent<IRaycastable>().HandleRaycast(this);
        }
    }

    private void TryJump()
    {
        if (isGrounded)
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityMultiplier);
    }


    #region Input Events
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
            TryJump();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
            TryInteract();
    }
    #endregion
}
