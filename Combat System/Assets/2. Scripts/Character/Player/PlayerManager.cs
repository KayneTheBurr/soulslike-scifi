using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerLocomotionManager playerLocomotionManager;



    protected override void Awake()
    {
        base.Awake();
        //do charcter stuff then player only stuff
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();

    }
    protected override void Start()
    {
        base.Start();
        PlayerCamera.instance.player = this;
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
