using UnityEngine;

public class BasicUndeadCombatManager : AICombatManager
{
    
    [Header("Damage Colliders")]
    [SerializeField] UndeadHandDamageCollider rightHandDamageCollider;
    [SerializeField] UndeadHandDamageCollider leftHandDamageCollider;

    [Header("Damage")]
    [SerializeField] float physicalDamage = 15;
    [SerializeField] float chemicalDamage = 10;
    [SerializeField] float attack01DamageModifier = 1f;
    [SerializeField] float attack02DamageModifier = 1.2f;
    [SerializeField] float swipeAttack01DamageModifier = 1.5f;

    public void SetAttack01Damage()
    {
        rightHandDamageCollider.physicalDamage = physicalDamage * attack01DamageModifier;
        rightHandDamageCollider.chemicalDamage = chemicalDamage * attack01DamageModifier;

        leftHandDamageCollider.physicalDamage = physicalDamage * attack01DamageModifier;
        leftHandDamageCollider.chemicalDamage = chemicalDamage * attack01DamageModifier;
    }

    public void SetAttack02Damage()
    {
        rightHandDamageCollider.physicalDamage = physicalDamage * attack02DamageModifier;
        rightHandDamageCollider.chemicalDamage = chemicalDamage * attack02DamageModifier;

        leftHandDamageCollider.physicalDamage = physicalDamage * attack02DamageModifier;
        leftHandDamageCollider.chemicalDamage = chemicalDamage * attack02DamageModifier;
    }

    public void SetSwipeAttack01Damage()
    {
        rightHandDamageCollider.physicalDamage = physicalDamage * swipeAttack01DamageModifier;
        rightHandDamageCollider.chemicalDamage = chemicalDamage * swipeAttack01DamageModifier;

        leftHandDamageCollider.physicalDamage = physicalDamage * swipeAttack01DamageModifier;
        leftHandDamageCollider.chemicalDamage = chemicalDamage * swipeAttack01DamageModifier;
    }

    public void OpenRightHandDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void CloseRightHandDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void OpenLeftHandDamageCollider()
    {
        leftHandDamageCollider.EnableDamageCollider();
    }

    public void CloseLeftHandDamageCollider()
    {
        leftHandDamageCollider.DisableDamageCollider();
    }

}
