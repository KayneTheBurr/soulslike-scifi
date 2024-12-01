using UnityEngine;


[CreateAssetMenu(menuName = "Character Actions/Weapon Action /Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public int actionID;



    public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        //we should always keep track of what weapon a player is using 
        if(playerPerformingAction.IsOwner)
        {
            playerPerformingAction.playerNetworkManager.currentWeaponBeingUsed.Value = weaponPerformingAction.itemID;
        }
        Debug.Log("play the action!");
    }


}
