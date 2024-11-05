using Mono.Cecil;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerLocomotionLogic : CharacterLocomotionLogic
{
    PlayerManager player;
    public float verticalMovement, horizontalMovement, moveAmount;
    private Vector3 movementDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkSpeed, runSpeed;
    [SerializeField] float rotationSpeed = 15;



    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    public void HandleAllMovmenet()
    {
        HandleGroundedMovement();
        HandleRotation();
    }
    private void GetVerticalAndHorizontalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
    }
    private void HandleGroundedMovement()
    {
        GetVerticalAndHorizontalInputs();
        movementDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        movementDirection = movementDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        movementDirection.y = 0;
        movementDirection.Normalize();
        

        if(PlayerInputManager.instance.moveAmount > 0.5f)
        {
            //run speed
            player.characterController.Move(runSpeed * Time.deltaTime * movementDirection);
        }
        else if(PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            //walk speed
            player.characterController.Move(walkSpeed * Time.deltaTime * movementDirection);
        }
    }
    private void HandleRotation()
    {
        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cam.transform.forward * verticalMovement;
        targetRotationDirection += PlayerCamera.instance.cam.transform.forward * horizontalMovement;
        targetRotationDirection.y = 0;
        targetRotationDirection.Normalize();

        if(targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }
}
