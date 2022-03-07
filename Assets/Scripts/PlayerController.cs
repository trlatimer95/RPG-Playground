using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float playerSpeed = 2.0f;
    public float jumpHeight = 3f;
    public float mouseSensitivity = 100f;
    public float gravityMultiplier = 3.0f;
    public float groundDistance = 0.04f;
    public LayerMask groundLayer;

    [Header("Interaction")]
    public int maxInteractableDistance = 10;
    public Text interactableText;

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
    private bool jumpInput = false;

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
        HandlePlayerInteraction();
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

            float xRotation = 0f;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.Rotate(Vector3.up * mouseX);
        }

        if (jumpInput && isGrounded)
            playerVelocity.y += jumpHeight; //Mathf.Sqrt(jumpHeight * -2f * gravityMultiplier);

        playerVelocity.y += gravityMultiplier * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HandlePlayerInteraction()
    {
        RaycastHit hit;
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * maxInteractableDistance, Color.white);
        if (Physics.Raycast(mainCamera.transform.position + new Vector3(0, 0, 0.2f), mainCamera.transform.TransformDirection(Vector3.forward), out hit, maxInteractableDistance) && hit.transform.gameObject.GetComponent<IRaycastable>() != null)
        {
            interactableText.text = hit.transform.gameObject.GetComponent<IRaycastable>().DisplayName();
            if (Input.GetMouseButtonDown(0))
            {
                hit.transform.gameObject.GetComponentInParent<IRaycastable>().HandleRaycast(this);
            }
        }
        else
        {
            interactableText.text = "";
        }
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
        jumpInput = context.action.triggered;
    }
    #endregion
}
