using Unity.Netcode;
using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimation;

    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    [Header("Attack Flags")]
    public bool canPerformRollingAttack = false;
    public bool canPerformBackStepAttack = false;

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
    public virtual void EnableCanDoCombo()
    {

    }
    public virtual void DisableCanDoCombo()
    {

    }

    public void EnableIFrames()
    {
        if (character.IsOwner)
            character.characterNetworkManager.isInvulnerable.Value = true;
    }
    public void DisableIFrames()
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
        canPerformRollingAttack = false;
    }
    public void EnableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = true;
    }
    public void DisableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = false;
    }
}
