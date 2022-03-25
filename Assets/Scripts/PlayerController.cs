using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    [Header("Look")]
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    private float xRotation = 0f;

    [Header("Interaction")]
    public int maxInteractableDistance = 10;
    public Text interactableText;
    private GameObject currentInteractableObject;

    [Header("Components")]
    public CharacterController controller;
    public Camera mainCamera;
    public Animator animator;

    [Header("Status")]
    public bool canMove = true;
    public bool sprinting = false;
    public bool jumping = false;
    Vector3 moveDirection = Vector3.zero;
    public Vector3 playerVelocity = Vector3.zero;

    // Input values
    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (controller == null)
            controller = gameObject.GetComponent<CharacterController>();
        if (mainCamera == null)
            mainCamera = gameObject.GetComponentInChildren<Camera>();
        if (animator == null)
            animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandlePlayerMovement();
        CheckForInteractable();
    }

    private void HandlePlayerMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Determine speeds and direction
        float curSpeedX = canMove ? (sprinting ? runningSpeed : walkingSpeed) * movementInput.y : 0;
        float curSpeedY = canMove ? (sprinting ? runningSpeed : walkingSpeed) * movementInput.x : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Update animation
        animator.SetFloat("inputX", curSpeedX);
        animator.SetFloat("inputY", curSpeedY);

        // Handle jumping
        if (jumping && canMove && controller.isGrounded)
            moveDirection.y = jumpSpeed;
        else
            moveDirection.y = movementDirectionY;

        // Apply gravity
        if (!controller.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);

        // Handle player look
        if (canMove)
        {
            xRotation += -lookInput.y * lookSpeed;
            xRotation = Mathf.Clamp(xRotation, -lookXLimit, lookXLimit);
            mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
        }
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

    //private void TryJump()
    //{
    //    if (isGrounded)
    //        playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityMultiplier);
    //}


    #region Input Events
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprinting = context.action.triggered;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumping = context.action.triggered;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
            TryInteract();
    }
    #endregion
}
