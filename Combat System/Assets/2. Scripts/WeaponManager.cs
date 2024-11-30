using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] MeleeWeaponDamageCollider meleeDamageCollider;

    private void Awake()
    {
        Debug.Log("no col");
        meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        Debug.Log("col!");
    }
    public void SetWeaponDamage(CharacterManager characterWithWeapon, WeaponItem weapon)
    {
        meleeDamageCollider.characterCausingDamage = characterWithWeapon;
        meleeDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeDamageCollider.plasmaDamage = weapon.plasmaDamage;
        meleeDamageCollider.electricalDamage = weapon.electricalDamage;
        meleeDamageCollider.chemicalDamage = weapon.chemicalDamage;
        meleeDamageCollider.cryoDamage = weapon.cryoDamage;
        meleeDamageCollider.radiationDamage = weapon.radiationDamage;
        meleeDamageCollider.geneticDamage = weapon.geneticDamage;
    }

}
