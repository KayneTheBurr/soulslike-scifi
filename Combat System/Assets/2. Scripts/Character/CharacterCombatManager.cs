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

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        if(lockOnTransform == null)
        {
            lockOnTransform = GetComponentInChildren<LockOnTarget>().GetComponent<Transform>();
        }
        
    }




}
