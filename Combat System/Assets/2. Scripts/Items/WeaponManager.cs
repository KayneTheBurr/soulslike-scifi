using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public MeleeWeaponDamageCollider meleeDamageCollider;

    private void Awake()
    {
        //Debug.Log("no col");
        meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        //Debug.Log("col!");
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

        meleeDamageCollider.light_Attack_01_DamageModifier = weapon.light_Attack_01_DamageModifier;
        meleeDamageCollider.light_Attack_02_DamageModifier = weapon.light_Attack_02_DamageModifier;
        meleeDamageCollider.heavy_Attack_01_DamageModifier = weapon.heavy_Attack_01_DamageModifier;
        meleeDamageCollider.heavy_Attack_02_DamageModifier = weapon.heavy_Attack_02_DamageModifier;
        meleeDamageCollider.charge_Attack_01_DamageModifier = weapon.charge_Attack_01_DamageModifier;
        meleeDamageCollider.charge_Attack_02_DamageModifier = weapon.charge_Attack_02_DamageModifier;
        meleeDamageCollider.light_Run_Attack_01_DamageModifier = weapon.light_Run_Attack_01_DamageModifier;
        meleeDamageCollider.light_Roll_Attack_01_DamageModifier = weapon.light_Roll_Attack_01_DamageModifier;
        meleeDamageCollider.light_BackStep_Attack_01_DamageModifier = weapon.light_BackStep_Attack_01_DamageModifier;

    }

}
