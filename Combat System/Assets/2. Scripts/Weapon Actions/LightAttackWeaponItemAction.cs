using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions /Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [Header("Light Attacks")]
    [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";
    [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";

    [Header("Light Run Attacks")]
    [SerializeField] string light_run_attack_01 = "SS_Main_Run_Attack_01";

    [Header("Light Dodge Attacks")]
    [SerializeField] string light_roll_attack_01 = "SS_Main_Roll_Attack_01";
    [SerializeField] string light_backstep_attack_01 = "SS_Main_Back_Step_Attack_01";

    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        //check for anything that stops the action
        if (!playerPerformingAction.IsOwner) return;

        //if out of stamina cant attack
        if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) return;

        //if player is not on the ground cant attack
        if (!playerPerformingAction.isGrounded) return;

        //if we are sprinting, perform a running attack 
        if(playerPerformingAction.characterNetworkManager.isSprinting.Value)
        {
            PerformLightRunningAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }
        //if we are rolling, perform a rolling attack
        if (playerPerformingAction.characterCombatManager.canPerformRollingAttack)
        {
            PerformLightRollingAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }
        //if we are backsteping, perform a backstep attack
        if (playerPerformingAction.characterCombatManager.canPerformBackStepAttack)
        {
            PerformLightBackStepAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are attacking and we can perform a combo, perform the combo attack
        if(playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            //figure out which attack we are currently performing, perform the next one
            //perform the attack based on the previous attack
            if(playerPerformingAction.playerCombatManager.lastAttackAnimation == light_Attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02, light_Attack_02, true);
            }
        
            else if(playerPerformingAction.playerCombatManager.lastAttackAnimation == light_Attack_02)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
            }
        }
        //otherwise perform a normal light attack
        else if(!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
        }

    }

    private void PerformLightRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are 2 handing, play 2 handing 
        //else perform one handed light running attack 

        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightRunningAttack01, light_run_attack_01, true);
        
    }
    private void PerformLightRollingAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are 2 handing, play 2 handing 
        //else perform one handed light rolling attack 

        playerPerformingAction.playerCombatManager.canPerformRollingAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightRollingAttack01, light_roll_attack_01, true);
        
    }
    private void PerformLightBackStepAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //if we are 2 handing, play 2 handing 
        //else perform one handed light rolling attack 

        playerPerformingAction.playerCombatManager.canPerformBackStepAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightBackStepAttack01, light_backstep_attack_01, true);

    }

}
