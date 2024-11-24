using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;

    public float verticalMovement, horizontalMovement, moveAmount;
    private Vector3 moveDirection;
    private Vector3 targetRotateDirection;
    public float walkSpeed, runSpeed, rotationSpeed;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public void AllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        //air movement
    }
    private void GetVertandHoriInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;

        //clamp for animations
    }
    private void HandleGroundedMovement()
    {
        GetVertandHoriInputs();

        //move direction is based on camera perspective and inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if(PlayerInputManager.instance.moveAmount > 0.5f)
        {
            //running speed
            player.characterController.Move(moveDirection * runSpeed * Time.deltaTime);
        }
        else if(PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            //walking
            player.characterController.Move(moveDirection * walkSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        targetRotateDirection = Vector3.zero;
        targetRotateDirection = PlayerCamera.instance.cam.transform.forward * verticalMovement;
        targetRotateDirection += PlayerCamera.instance.cam.transform.right * horizontalMovement;
        targetRotateDirection.Normalize();
        targetRotateDirection.y = 0;


        if(targetRotateDirection == Vector3.zero)
        {
            targetRotateDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotateDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

}
