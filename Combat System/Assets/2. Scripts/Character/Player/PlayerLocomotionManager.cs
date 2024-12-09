using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;

    [HideInInspector] public float verticalMovement, horizontalMovement, moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotateDirection;
    [SerializeField] float rotationSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] public float walkSpeed;
    public float sprintStaminaCost;

    [Header("Jump")]
    [SerializeField] float jumpStaminaCost = 15;
    [SerializeField] float jumpHeight = 2;
    private Vector3 jumpDirection;
    [SerializeField] float jumpForwardSpeed = 5;
    [SerializeField] float airborneManeuverSpeed = 3;

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
        if(player.IsOwner)
        {
            player.characterNetworkManager.verticalMovement.Value = verticalMovement;
            player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
            player.characterNetworkManager.moveAmount.Value = moveAmount;
        }
        else
        {
            verticalMovement = player.characterNetworkManager.verticalMovement.Value;
            horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
            moveAmount = player.characterNetworkManager.moveAmount.Value;

            if (player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalMovement, verticalMovement, player.playerNetworkManager.isSprinting.Value);
            }
            else
            {
                //only pass vertical since strafing is only while locked on, want to only run forward with camera movement right now 
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }


        }

    }
    public void AllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleAirborneMovement();
    }
    private void GetVertandHoriInputs()
    {
        verticalMovement = PlayerInputManager.instance.vertical_Input;
        horizontalMovement = PlayerInputManager.instance.horizontal_Input;
        moveAmount = PlayerInputManager.instance.moveAmount;

        //clamp for animations
    }
    private void HandleGroundedMovement()
    {
        if(player.canMove || player.canRotate)
        {
            GetVertandHoriInputs();
        }

        if (!player.canMove) return; //if i cant move, dont let me move

        //move direction is based on camera perspective and inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if(player.playerNetworkManager.isSprinting.Value)
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

    private void HandleJumpingMovement()
    {
        if(player.playerNetworkManager.isJumping.Value)
        {
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void HandleAirborneMovement()
    {
        if(!player.isGrounded)
        {
            Vector3 airborneDirection;
            airborneDirection = PlayerCamera.instance.cam.transform.forward * PlayerInputManager.instance.vertical_Input;
            airborneDirection += PlayerCamera.instance.cam.transform.right * PlayerInputManager.instance.horizontal_Input;
            airborneDirection.y = 0;

            player.characterController.Move(airborneDirection * airborneManeuverSpeed * Time.deltaTime);

        }
    }

    private void HandleRotation()
    {
        if (player.isDead.Value) return;
        if (!player.canRotate) return; //if i cant rotate, dont let me rotate

        if (player.playerNetworkManager.isLockedOn.Value)//if locked on 
        {
            //allow sprinting/rolling without facing the target while locked on
            if (player.playerNetworkManager.isSprinting.Value || player.playerLocomotionManager.isRolling) 
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = PlayerCamera.instance.cam.transform.forward * verticalMovement;
                targetDirection += PlayerCamera.instance.cam.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
            else //if locked on and not sprinting, we need to be strafing instead of normal movement 
            {
                if (player.playerCombatManager.currentTarget == null) return;

                Vector3 targetDirection;
                targetDirection = player.playerCombatManager.currentTarget.transform.position - player.transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();

                Debug.DrawLine(player.playerCombatManager.currentTarget.transform.position, player.transform.position, Color.red);

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                player.transform.rotation = finalRotation;

            }
        }
        else
        {
            targetRotateDirection = Vector3.zero;
            targetRotateDirection = PlayerCamera.instance.cam.transform.forward * verticalMovement;
            targetRotateDirection += PlayerCamera.instance.cam.transform.right * horizontalMovement;
            targetRotateDirection.Normalize();
            targetRotateDirection.y = 0;

            if (targetRotateDirection == Vector3.zero)
            {
                targetRotateDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotateDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            //if doing something else, set sprinting to false
            player.playerNetworkManager.isSprinting.Value = false;
        }
        //if out of stamina stop sprinting
        if(player.playerNetworkManager.currentStamina.Value <= 0)
        {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }

        if(moveAmount >= 0.5f)
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
        if(player.playerNetworkManager.isSprinting.Value)
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

            isRolling = true;
        }
        else //if stationary, dodge backwards (backstep)
        {
            if(player.isGrounded) //roll allowed in air, backstep is not 
            {
                player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true);

                player.playerNetworkManager.currentStamina.Value -= backStepStaminaCost;
            }
        }
    }
    public void AttemptToJump()
    {
        //no jumping is doing another action (can be changed to allow attacks)
        if (player.isPerformingAction) return;

        //no jump if out of stamina
        if (player.playerNetworkManager.currentStamina.Value <= 0) return;

        //no jump if we are already jumping
        if (player.playerNetworkManager.isJumping.Value) return;

        //no jumping if we are not on the ground
        if (!player.isGrounded) return;

        //play animation depending on which weapon/how many weapons we are using etc
        player.playerAnimatorManager.PlayTargetActionAnimation("SS_Main_Jump_Start_01", false, true);
        player.playerNetworkManager.isJumping.Value = true;

        player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

        jumpDirection = PlayerCamera.instance.cam.transform.forward * PlayerInputManager.instance.vertical_Input;
        jumpDirection += PlayerCamera.instance.cam.transform.right * PlayerInputManager.instance.horizontal_Input;
        jumpDirection.y = 0;
        
        if(jumpDirection != Vector3.zero)
        {
            //our movement speed will affect how much we can move while in the air 
            if (player.playerNetworkManager.isSprinting.Value)
            {
                jumpDirection *= 2;
            }
            else if (moveAmount > 0.5f)
            {
                jumpDirection *= 1f;
            }
            else if (moveAmount <= 0.5f)
            {
                jumpDirection *= 0.25f;
            }
        }
    }

    public void ApplyJumpForce()
    {
        
        //apply an upward velocity, depends on in game forces
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);

    }

}
