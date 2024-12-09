using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    public WeaponItem currentWeaponBeingUsed;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;
    //public bool canComboWithOffHandWeapon = false;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        if(player.IsOwner)
        {
            //perform the action here
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);

            //also perform action on other netwoerk ppl world (clients)
            player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(
                NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
        }
    }

    public override void EnableCanDoCombo()
    {
        base.EnableCanDoCombo();
        if(player.playerNetworkManager.isUsingRightHand.Value)
        {
            canComboWithMainHandWeapon = true;
        }
        else
        {
            //if/when enabling left handed attacks and combos
            //canComboWithOffHandWeapon = true;
        }
    }

    public override void DisableCanDoCombo()
    {
        base.DisableCanDoCombo();
        canComboWithMainHandWeapon = false;
        //canComboWithOffHandWeapon = false; //added for when/if off hand dual wield attacks potentially?

    }

    public void DrainStaminaBasedOnAttack()
    {
        
        if(!player.IsOwner) return;
        if (currentWeaponBeingUsed == null) return;
        
        float staminaDrained = 0f;

        switch(currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostModifier;
                break;
            case AttackType.LightAttack02:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostModifier;
                break;
            case AttackType.HeavyAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostModifier;
                break;
            case AttackType.HeavyAttack02:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostModifier;
                break;
            case AttackType.ChargeAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostModifier;
                break;
            case AttackType.ChargeAttack02:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostModifier;
                break;
            case AttackType.LightRunningAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightRunAttackStaminaCostModifier;
                break;
            case AttackType.LightRollingAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightRollAttackStaminaCostModifier;
                break;
            case AttackType.LightBackStepAttack01:
                staminaDrained = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightBackStepAttackStaminaCostModifier;
                break;
            default:
                break;
        }

        //Debug.Log("Stamina Drained" + staminaDrained);

        player.playerNetworkManager.currentStamina.Value -= staminaDrained;
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        if( player.IsOwner )
        {
            PlayerCamera.instance.SetLockOnCameraHeight();
        }

    }
}
