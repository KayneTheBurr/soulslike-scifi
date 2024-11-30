using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{

    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage; //lets us check for the attackers damage modifiers etc 

}
