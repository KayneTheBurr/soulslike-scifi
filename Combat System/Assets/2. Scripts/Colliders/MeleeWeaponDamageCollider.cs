using Unity.Netcode;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{

    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage; //lets us check for the attackers damage modifiers etc 

    [Header("Weapon Attack Modifiers")]
    public float light_Attack_01_DamageModifier;


    protected override void Awake()
    {
        base.Awake();

        if(damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }

        //melee damage colliders only on during attack animations 
        damageCollider.enabled = false;
    }

    protected override void OnTriggerEnter(Collider col)
    {
        CharacterManager damageTarget = col.GetComponentInParent<CharacterManager>();

        if (damageTarget != null)
        {
            if (damageTarget == characterCausingDamage) return; //dont let us hit ourselves with our attacks 

            contactPoint = damageTarget.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            //check if we can damage this target or not based on characters "freindly fire" 
            //check if target is blocking
            //check if target is invulnerable
            DamageTarget(damageTarget);

        }
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        //dont want to deal damage again to a target if we already damaged them with this instance of damage
        //add them to a list and check the list to see if they are on the list of damageable characters already or not 
        if (charactersDamaged.Contains(damageTarget)) return;
        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.plasmaDamage = plasmaDamage;
        damageEffect.electricalDamage = electricalDamage;
        damageEffect.cryoDamage = cryoDamage;
        damageEffect.chemicalDamage = chemicalDamage;
        damageEffect.radiationDamage = radiationDamage;
        damageEffect.geneticDamage = geneticDamage;

        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);
        Debug.Log(damageEffect.angleHitFrom);

        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(light_Attack_01_DamageModifier, damageEffect);
                break;
            default:
                break;
        }

        //only send the damage request from the damage collider owner, otehrwise it could then trigger the damage 
        if(characterCausingDamage.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyServerOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId, characterCausingDamage.NetworkObjectId, damageEffect.physicalDamage,
                damageEffect.plasmaDamage, damageEffect.electricalDamage, damageEffect.cryoDamage, damageEffect.chemicalDamage,
                damageEffect.radiationDamage, damageEffect.geneticDamage, damageEffect.poiseDamage,
                damageEffect.angleHitFrom, damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
        }
    }

    private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
    {
        damage.physicalDamage *= modifier;
        damage.plasmaDamage *= modifier;
        damage.electricalDamage *= modifier;
        damage.cryoDamage *= modifier;
        damage.chemicalDamage *= modifier;
        damage.radiationDamage *= modifier;
        damage.geneticDamage *= modifier;
        
        damage.poiseDamage *= modifier;

        //if the attack is fully charged, multiply by full charge modifier AFTER normal modifiers 

    }
}
