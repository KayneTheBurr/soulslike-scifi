using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;

    [Header("Damage Types")]
    public float physicalDamage = 0; //break down into sub types (standard, slash, pierce, strike)
    public float plasmaDamage = 0;
    public float electricalDamage = 0;
    public float cryoDamage = 0;
    public float chemicalDamage = 0;
    public float geneticDamage = 0;
    public float radiationDamage = 0;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    protected virtual void Awake()
    {
        
    }
    protected virtual void OnTriggerEnter(Collider col)
    {
        CharacterManager damageTarget = col.GetComponentInParent<CharacterManager>();
        Debug.Log("hit");

        if (damageTarget != null )
        {
            contactPoint = damageTarget.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            //check if we can damage this target or not based on characters "freindly fire" 
            //check if target is blocking

            //check if target is invulnerable
            if (damageTarget.characterNetworkManager.isInvulnerable.Value) return;

            DamageTarget(damageTarget);

            Debug.Log(damageTarget);
        }
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
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

        damageTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }
    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear(); //reset the characters list so you can damage characters on the next attack
    }
}
