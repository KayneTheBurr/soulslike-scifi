using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical;
    int horizontal;

    [Header("Damage Animations")]
    public string lastDamageAnimationPlayed;

    [SerializeField] string hit_Fwd_Med_01 =  "SS_hit_Fwd_Med_01";
    [SerializeField] string hit_Fwd_Med_02 = "SS_hit_Fwd_Med_02";

    [SerializeField] string hit_Back_Med_01 = "SS_hit_Back_Med_01";
    [SerializeField] string hit_Back_Med_02 = "SS_hit_Back_Med_02";

    [SerializeField] string hit_Left_Med_01 = "SS_hit_Left_Med_01";
    [SerializeField] string hit_Left_Med_02 = "SS_hit_Left_Med_02";

    [SerializeField] string hit_Right_Med_01 = "SS_hit_Right_Med_01";
    [SerializeField] string hit_Right_Med_02 = "SS_hit_Right_Med_02";

    public List<string> Fwd_Med_Damage = new List<string>();
    public List<string> Back_Med_Damage = new List<string>();
    public List<string> Left_Med_Damage = new List<string>();
    public List<string> Right_Med_Damage = new List<string>();

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");

    }

    protected virtual void Start()
    {
        Fwd_Med_Damage.Add(hit_Fwd_Med_01);
        Fwd_Med_Damage.Add(hit_Fwd_Med_02);

        Back_Med_Damage.Add(hit_Back_Med_01);
        Back_Med_Damage.Add(hit_Back_Med_02);

        Left_Med_Damage.Add(hit_Left_Med_01);
        Left_Med_Damage.Add(hit_Left_Med_02);

        Right_Med_Damage.Add(hit_Right_Med_01);
        Right_Med_Damage.Add(hit_Right_Med_02);
    }

    public string GetRandomAnimationFromList(List<string> animationList)
    {
        List<string> finalList = new List<string>();
        foreach(var anim in animationList)
        {
            finalList.Add(anim);
        }
        //check if we have played this animation so we dont play it twice in a row 
        finalList.Remove(lastDamageAnimationPlayed);

        //check list for null entries and remove them 
        for (int i = finalList.Count-1 ; i > -1 ; i--)
        {
            if (finalList[i] == null)
            {
                finalList.RemoveAt(i);
            }
        }

        int randomValue = Random.Range(0, finalList.Count);

        return finalList[randomValue];

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
        Debug.Log(hit_Fwd_Med_01);
        Debug.Log("playing animation:" +  targetAnimation);

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
