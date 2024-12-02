using UnityEngine;
using Unity.Netcode;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical, horizontal;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");

    }
    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
    {
        float horizontalAmt = horizontalValue;
        float verticalAmt = verticalValue;

        if(isSprinting )
        {
            verticalAmt = 2;
        }
        character.animator.SetFloat(horizontal, horizontalAmt, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, verticalAmt, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, 
                            bool isPerformingAction, bool applyRootMotion = true,
                            bool canRotate = false, bool canMove = false)
    {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);

        //stop character from performing other actions
        character.isPerformingAction = isPerformingAction;
        character.canMove = canMove;
        character.canRotate = canRotate;

        //tell the server/host we played an animation, tell everyone elese 
        character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

    public virtual void PlayTargetAttackActionAnimation( AttackType attackType,
                            string targetAnimation,
                            bool isPerformingAction, bool applyRootMotion = true,
                            bool canRotate = false, bool canMove = false)
    {
        //keep track of the last attack performed
        //keep track of current attack type
        //update the animation set to current weapon animations
        //decide if our attack can be parried
        //tell the network we are attacking flag (counter damage etc)

        character.characterCombatManager.currentAttackType = attackType;
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;
        character.canMove = canMove;
        character.canRotate = canRotate;

        //tell the server/host we played an animation, tell everyone elese 
        character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(
            NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

}
