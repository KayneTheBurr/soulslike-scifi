using UnityEngine;

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
            //player.playerNetworkManager.NotifyServerOfActionAnimationServerRpc
        }

    }



}
