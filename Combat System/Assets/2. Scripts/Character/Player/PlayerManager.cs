using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;

    protected override void Awake()
    {
        base.Awake();
        //do charcter stuff then player only stuff
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
    }
    protected override void Start()
    {
        base.Start();
        PlayerCamera.instance.player = this;
        PlayerInputManager.instance.player = this;

        //read stamina value, set stamina any time it is changed
        playerNetworkManager.currentStamina.OnValueChanged = PlayerUIManager.instance.playerHUDManager.SetNewStaminaValue;

        playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEndurance(playerNetworkManager.endurance.Value);
        PlayerUIManager.instance.playerHUDManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);



    }
    protected override void Update()
    {
        base.Update();

        playerLocomotionManager.AllMovement();

    }
    protected override void LateUpdate()
    {
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraAction();

    }



}
