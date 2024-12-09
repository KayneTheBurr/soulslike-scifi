using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    PlayerControls playerControls;
    public PlayerManager player;

    [Header("Camera Inputs")]
    [SerializeField] Vector2 camera_Input;
    public float camVerticalInput;
    public float camHorizontalInput;

    [Header("Lock On")]
    [SerializeField] bool lockOn_Input = false;
    [SerializeField] bool lockOn_Left_Input = false;
    [SerializeField] bool lockOn_Right_Input = false;
    private Coroutine lockOnCoroutine;

    [Header("Movement Inputs")]
    [SerializeField] Vector2 movement_Input;
    public float vertical_Input;
    public float horizontal_Input;
    public float moveAmount;

    [Header("Player Action Inputs")]
    [SerializeField] bool dodge_Input = false;
    [SerializeField] bool sprint_Input = false;
    [SerializeField] bool jump_Input = false;

    [Header("Qued Inputs")]
    [SerializeField] float que_Input_Timer = 0;
    [SerializeField] float default_Que_Input_Timer = 0.5f;
    [SerializeField] bool input_Que_Active = false;
    [SerializeField] bool qued_r1_input = false;
    [SerializeField] bool qued_r2_input = false;

    [Header("Bumper (R1/L1) Inputs")]
    [SerializeField] bool r1_Input = false;

    [Header("Trigger (R2/L2) Inputs")]
    [SerializeField] bool r2_Input = false;
    [SerializeField] bool charge_r2_Input = false;

    [Header("Quick Slot Inputs")]
    [SerializeField] bool switch_Right_Weapon_Input = false;
    [SerializeField] bool switch_Left_Weapon_Input = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //do this once when scene changes

    }
    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldSaveGameManager.instance.worldSceneIndex)
        {
            instance.enabled = true;
            if (playerControls != null)
            {
                playerControls.Enable();
            }
        }
        else
        {
            instance.enabled = false;
            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movement_Input = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.CameraControls.performed += i => camera_Input = i.ReadValue<Vector2>();

            //mobility actions
            playerControls.PlayerActions.Dodge.performed += i => dodge_Input = true;
            playerControls.PlayerActions.Jump.performed += i => jump_Input = true;

            //r1/l1 bumpers
            playerControls.PlayerActions.R1LeftClick.performed += i => r1_Input = true;

            //r2/l2 triggers 
            playerControls.PlayerActions.R2.performed += i => r2_Input = true;
            playerControls.PlayerActions.ChargeR2.performed += i => charge_r2_Input = true;
            playerControls.PlayerActions.ChargeR2.canceled += i => charge_r2_Input = false;

            //Change Equipment/Items
            playerControls.PlayerActions.SwitchRightWeapon.performed += i => switch_Right_Weapon_Input = true;
            playerControls.PlayerActions.SwitchLeftWeapon.performed += i => switch_Left_Weapon_Input = true;

            //Lock on Inputs
            playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
            playerControls.PlayerActions.SeekLockOnTargetLeft.performed += i => lockOn_Left_Input = true;
            playerControls.PlayerActions.SeekLockOnTargetRight.performed += i => lockOn_Right_Input = true;

            //Sprint Holds
            playerControls.PlayerActions.Sprint.performed += i => sprint_Input = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprint_Input = false;

            //Qued Inputs
            playerControls.PlayerActions.QuedR1.performed += i => QuedInput(ref qued_r1_input);
            playerControls.PlayerActions.QuedR2.performed += i => QuedInput(ref qued_r2_input);

        }

        playerControls.Enable();
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChange;
        instance.enabled = false;

        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }
    private void Update()
    {
        HandleAllInputs();
    }
    private void HandleAllInputs()
    {
        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();
        HandleSwitchRightWeaponSlot();
        HandleSwitchLeftWeaponSlot();
        HandleR1Input();
        HandleR2Input();
        HandleChargeR2Input();
        HandleQuedInputs();
    }

    //Lock On
    private void HandleLockOnInput()
    {
        //check for dead target
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentTarget == null) return;
            if (player.playerCombatManager.currentTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;
            }

            //try to find new target to lock onto, makes sure the coroutine can not be running more than one at a time 
            if (lockOnCoroutine != null)
            {
                StopCoroutine(lockOnCoroutine);
            }
            lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
        }

        //if already locked on, unlock on from targets
        if (lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;
            PlayerCamera.instance.ClearLockOnTargets();
            player.playerNetworkManager.isLockedOn.Value = false;


            return;
        }
        if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;

            //how does lock on affect ranged weapons?

            PlayerCamera.instance.HandleLocatingLockOnTargets();

            if (PlayerCamera.instance.nearestLockOnTarget != null)
            {
                //set the target ar our current lock on target 
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }
        }
    }
    private void HandleLockOnSwitchTargetInput()
    {
        if (lockOn_Left_Input)
        {
            lockOn_Left_Input = false;
            if (player.playerNetworkManager.isLockedOn.Value) //check if already locked on something
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();
                if (PlayerCamera.instance.leftLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }
        if (lockOn_Right_Input)
        {
            lockOn_Right_Input = false;
            if (player.playerNetworkManager.isLockedOn.Value) //check if already locked on something
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();
                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }
    }

    //Movements
    private void HandlePlayerMovementInput()
    {
        vertical_Input = movement_Input.y;
        horizontal_Input = movement_Input.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

        //clamped movement in 2 different speeds
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1.0f;
        }

        if (player == null) return;

        if (moveAmount != 0)
        {
            player.playerNetworkManager.isMoving.Value = true;
        }
        else
        {
            player.playerNetworkManager.isMoving.Value = false;
        }

        if (player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontal_Input, vertical_Input, player.playerNetworkManager.isSprinting.Value);
        }
        else
        {
            //only pass vertical since strafing is only while locked on, want to only run forward with camera movement right now 
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
        }
    }

    private void HandleCameraMovementInput()
    {
        camVerticalInput = camera_Input.y;
        camHorizontalInput = camera_Input.x;
    }

    //Actions
    private void HandleDodgeInput()
    {
        if (dodge_Input)
        {
            dodge_Input = false;
            //disable when menu is open
            //check for stamina
            player.playerLocomotionManager.AttemptToDodge();
        }
    }

    private void HandleSprintInput()
    {
        if (sprint_Input)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }

    public void HandleJumpInput()
    {
        if (jump_Input)
        {
            jump_Input = false;

            //dont do this if a menu is open

            //check for grounded status or performing other actions

            player.playerLocomotionManager.AttemptToJump();
        }
    }

    private void HandleSwitchRightWeaponSlot()
    {
        if (switch_Right_Weapon_Input)
        {
            switch_Right_Weapon_Input = false;
            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }

    private void HandleSwitchLeftWeaponSlot()
    {
        if (switch_Left_Weapon_Input)
        {
            switch_Left_Weapon_Input = false;
            player.playerEquipmentManager.SwitchLeftWeapon();
        }
    }

    //Attacks
    private void HandleR1Input()
    {
        if (r1_Input)
        {
            r1_Input = false;
            //do nothing if UI window is open 

            player.playerNetworkManager.SetCharacterActionHand(true);

            //if we are using 2 hands, do the 2hand action instead of one hand action 
            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_r1_Action,
                player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleR2Input()
    {
        if (r2_Input)
        {
            r2_Input = false;

            //do nothing if UI window is open 

            player.playerNetworkManager.SetCharacterActionHand(true);

            //if we are using 2 hands, do the 2hand action instead of one hand action 
            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_r2_Action,
                player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleChargeR2Input()
    {
        if (player.isPerformingAction)
        {
            //only want to check this if we are performing an action already that requires charging 
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                
                player.playerNetworkManager.isChargingAttack.Value = charge_r2_Input;
            }
        }
    }

    private void QuedInput(ref bool quedInput) //passing a ref means we pass a specific bool, and not the value of the bool(True or false)
    {
        //reset all qued inputs, only one at a time allowed
        qued_r1_input = false;
        qued_r2_input = false;
        //qued_l1_input = false;
        //qued_l2_input = false;

        //check for UI window being open, return if so 

        if (player.isPerformingAction || player.playerNetworkManager.isJumping.Value)
        {
            quedInput = true;
            que_Input_Timer = default_Que_Input_Timer;
            input_Que_Active = true;
        }
    }

    private void ProcessQuedInputs()
    {
        if (player.isDead.Value) return;

        if (qued_r1_input) r1_Input = true;
        if (qued_r2_input) r2_Input = true;
    }

    private void HandleQuedInputs()
    {
        if(input_Que_Active)
        {
            //while timer is > 0, keep attempting the input press
            if(que_Input_Timer > 0)
            {
                que_Input_Timer -= Time.deltaTime;
                ProcessQuedInputs();
            }
            else
            {
                qued_r1_input = false;
                qued_r2_input = false;
                //qued_l1_input = false;
                //qued_l2_input = false;
                input_Que_Active = false;
                que_Input_Timer = 0;
            }
        }
    }


}

    
