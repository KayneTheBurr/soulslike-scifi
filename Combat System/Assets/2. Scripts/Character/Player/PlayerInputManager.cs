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

    [Header("Movement Inputs")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput, horizontalInput, moveAmount;

    [Header("Player Action Inputs")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;


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
        }
        else
        {
            instance.enabled = false;
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
    }

    private void Update()
    {
        HandleAllInputs();
    }

    private void HandleAllInputs()
    {
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();
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

        if (player == null)
            return;

        //only pass vertical since strafing is only while locked on, want to only run forward with camera movement right now 
        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.isSprinting);


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
            player.isSprinting = false;
        }
    }

    private void HandleJumpInput()
    {
        if(jumpInput)
        {
            jumpInput = false;

            //dont do this if a menu is open

            //check for grounded status or performing other actions

            player.playerLocomotionManager.AttemptToJump();
        }
    }
    
}
