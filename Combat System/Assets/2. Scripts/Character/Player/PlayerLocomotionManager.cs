using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;

    [HideInInspector] public float verticalMovement, horizontalMovement, moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotateDirection;
    public float walkSpeed, runSpeed, sprintSpeed, rotationSpeed;
    public float sprintStaminaCost;

    [Header("Dodge")]
    private Vector3 dodgeDirection;
    [SerializeField] float dodgeStaminaCost = 25;
    [SerializeField] float backStepStaminaCost = 15;




    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    protected override void Update()
    {
        base.Update();


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
        moveAmount = PlayerInputManager.instance.moveAmount;

        //clamp for animations
    }
    private void HandleGroundedMovement()
    {
        GetVertandHoriInputs();

        if (!player.canMove) return; //if i cant move, dont let me move

        //move direction is based on camera perspective and inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if(player.isSprinting)
        {
            player.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
        }
        else
        {
            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                //running speed
                player.characterController.Move(moveDirection * runSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                //walking
                player.characterController.Move(moveDirection * walkSpeed * Time.deltaTime);
            }
        }

    }

    private void HandleRotation()
    {
        if (!player.canRotate) return; //if i cant rotate, dont let me rotate

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

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            //if doing something else, set sprinting to false
            player.isSprinting = false;
        }
        //if out of stamina stop sprinting
        if(player.playerNetworkManager.currentStamina.Value <= 0)
        {
            player.isSprinting = false;
            return;
        }

        if(moveAmount >= 0.5f)
        {
            player.isSprinting = true;
        }
        else
        {
            player.isSprinting = false;
        }
        if(player.isSprinting)
        {
            player.playerNetworkManager.currentStamina.Value -= sprintStaminaCost * Time.deltaTime;
        }
    }

    public void AttemptToDodge()
    {
        //check for other actions and stamina
        if (player.isPerformingAction) return;
        if (player.playerNetworkManager.currentStamina.Value <= 0) return;

        if (moveAmount > 0) //if moving, dodge in direction of movement
        {
            
            dodgeDirection = PlayerCamera.instance.cam.transform.forward * verticalMovement;
            dodgeDirection += PlayerCamera.instance.cam.transform.right * horizontalMovement;
            dodgeDirection.y = 0;
            dodgeDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(dodgeDirection);
            player.transform.rotation = playerRotation;

            player.playerAnimatorManager.PlayTargetActionAnimation("Fwd_Dodge_01", true, true);

            player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;

        }
        else //if stationary, dodge backwards (backstep)
        {
            player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true);

            player.playerNetworkManager.currentStamina.Value -= backStepStaminaCost;
        }
        
    }




}
