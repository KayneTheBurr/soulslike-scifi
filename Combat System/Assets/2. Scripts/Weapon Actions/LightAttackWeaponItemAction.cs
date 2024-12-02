using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions /Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";

    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        //check for anything that stops the action
        if (!playerPerformingAction.IsOwner) return;

        //if out of stamina cant attack
        if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) return;

        //if player is not on the ground cant attack
        if (!playerPerformingAction.isGrounded) return;

        //if player is performing another action, cant attack 
        if(playerPerformingAction.isPerformingAction) return;

        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        

        if(playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
        }
        if(playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
        {

        }
    }



}
