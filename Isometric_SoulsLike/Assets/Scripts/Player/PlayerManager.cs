using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerLocomotionLogic playerLocomotion;




    protected override void Awake()
    {
        base.Awake();
        playerLocomotion = GetComponent<PlayerLocomotionLogic>();

    }
    protected override void Update()
    {
        base.Update();

        //dont control this if we dont own it 
        if(!IsOwner)
        {
            return;
        }
        //do movement things
        playerLocomotion.HandleAllMovmenet();
    }


}
