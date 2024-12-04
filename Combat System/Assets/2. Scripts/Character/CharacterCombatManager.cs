using Unity.Netcode;
using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    [Header("Attack Flags")]
    public bool canPerformRollingAttack = false;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        if(lockOnTransform == null)
        {
            lockOnTransform = GetComponentInChildren<LockOnTarget>().GetComponent<Transform>();
        }
        
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if(character.IsOwner)
        {
            if(newTarget != null)
            {
                currentTarget = newTarget;
                //tell server the ulong ID of the target we locked onto 
                character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                
            }
            else
            {
                currentTarget = null;
            }
        }
    }

    public void EnableIsInvulnerable()
    {
        if (character.IsOwner)
            character.characterNetworkManager.isInvulnerable.Value = true;
    }
    public void DisableIsInvulnerable()
    {
        if (character.IsOwner)
            character.characterNetworkManager.isInvulnerable.Value = false;
    }
    public void EnableCanDoRollingAttack()
    {
        canPerformRollingAttack = true;
    }
    public void DisableCanDoRollingAttack()
    {
        canPerformRollingAttack = true;
    }
}
