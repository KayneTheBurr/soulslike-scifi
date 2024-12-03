using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    public WeaponItem currentWeaponBeingUsed;
    


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
            default:
                break;
        }

        Debug.Log("Stamina Drained" + staminaDrained);

        player.playerNetworkManager.currentStamina.Value -= staminaDrained;
    }


}
