using UnityEngine;

public class UndeadHandDamageCollider : DamageCollider
{
    public AICharacterManager undeadCharacter;

    protected override void Awake()
    {
        base.Awake();
        damageCollider = GetComponent<Collider>();
        undeadCharacter = GetComponentInParent<AICharacterManager>();
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
        damageEffect.angleHitFrom = Vector3.SignedAngle(undeadCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);



        //only send the damage request from the damage collider owner, otehrwise it could then trigger the damage a second time 
        if (undeadCharacter.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyServerOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId, undeadCharacter.NetworkObjectId, damageEffect.physicalDamage,
                damageEffect.plasmaDamage, damageEffect.electricalDamage, damageEffect.cryoDamage, damageEffect.chemicalDamage,
                damageEffect.radiationDamage, damageEffect.geneticDamage, damageEffect.poiseDamage,
                damageEffect.angleHitFrom, damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
        }
    }

}
