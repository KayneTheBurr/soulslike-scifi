using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    PlayerControls playerControls;
    public PlayerManager player;

    [Header("Camera Inputs")]
    [SerializeField] Vector2 cameraInput;
    public float camVerticalInput, camHorizontalInput;

    [Header("Lock On")]
    [SerializeField] bool lockOn_Input = false;

    [Header("Movement Inputs")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput, horizontalInput, moveAmount;

    [Header("Player Action Inputs")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] bool r1_Input = false;

    

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

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.CameraControls.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.R1LeftClick.performed += i => r1_Input = true;
            playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;

            //hold to sprint, release to cancel
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
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

        if(playerControls != null)
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
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();
        HandleR1Input();
    }
    
    //Lock On
    private void HandleLockOnInput()
    {
        //check for dead target
        if(player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentTarget == null) return;
            if(player.playerCombatManager.currentTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;
            }
            //try to find new target to lock onto 
        }

        //if already locked on, unlock on from targets
        if(lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;
            PlayerCamera.instance.ClearLockOnTargets();
            player.playerNetworkManager.isLockedOn.Value = false;

            
            return;
        }
        if(lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;

            //how does lock on affect ranged weapons?

            PlayerCamera.instance.HandleLocatingLockOnTargets();

            if(PlayerCamera.instance.nearestLockOnTarget != null)
            {
                //set the target ar our current lock on target 
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }
        }
    }
    
    
    //Movements
    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

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
        
        if(moveAmount != 0)
        {
            player.playerNetworkManager.isMoving.Value = true;
        }
        else
        {
            player.playerNetworkManager.isMoving.Value = false;
        }

        //only pass vertical since strafing is only while locked on, want to only run forward with camera movement right now 
        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);


    }
    private void HandleCameraMovementInput()
    {
        camVerticalInput = cameraInput.y;
        camHorizontalInput = cameraInput.x;
    }
    //Actions
    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;
            //disable when menu is open
            //check for stamina
            player.playerLocomotionManager.AttemptToDodge();
        }
    }

    private void HandleSprintInput()
    {
        if (sprintInput)
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
        if(jumpInput)
        {
            jumpInput = false;

            //dont do this if a menu is open

            //check for grounded status or performing other actions

            player.playerLocomotionManager.AttemptToJump();
        }
    }
    
    private void HandleR1Input()
    {
        if(r1_Input)
        {
            r1_Input = false;
            //do nothing if UI window is open 

            player.playerNetworkManager.SetCharacterActionHand(true);

            //if we are using 2 hands, do the 2hand action instead of one hand action 
            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_r1_Action,
                player.playerInventoryManager.currentRightHandWeapon);



        }
    }

}
