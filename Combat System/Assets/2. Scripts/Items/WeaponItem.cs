using UnityEngine;

public class WeaponItem : Item
{
    //animator controller override (change attack animations based on what weapon you are currently using)

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Stat Requirements")]
    public int strREQ = 0;
    public int dexREQ = 0;
    public int intREQ = 0;
    public int wisREQ = 0;
    public int chaosREQ = 0;
    public int perceptionREQ = 0;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int plasmaDamage = 0;
    public int cryoDamage = 0;
    public int chemicalDamage = 0;
    public int radiationDamage = 0;
    public int electricalDamage = 0;
    public int geneticDamage = 0;

    [Header("Attack Modifiers")]
    //stat scaling
    //weapon modifiers
    public float light_Attack_01_DamageModifier = 0.8f;
    public float light_Attack_02_DamageModifier = 0.9f;
    public float heavy_Attack_01_DamageModifier = 1.3f;
    public float heavy_Attack_02_DamageModifier = 1.4f;
    public float charge_Attack_01_DamageModifier = 2.0f;
    public float charge_Attack_02_DamageModifier = 2.2f;
    public float light_Run_Attack_01_DamageModifier = 0.75f;
    public float light_Roll_Attack_01_DamageModifier = 1.0f;
    public float light_BackStep_Attack_01_DamageModifier = 1.4f;
    
    //critical modifiers

    [Header("Weapon Base Poise Damage")]
    public float poiseDamage = 10;
    //weapon hyperarmor?

    //weapon blocking absorbtion and stagger

    [Header("Stamina Cost Modifiers")]
    public int baseStaminaCost = 20;
    public float lightAttackStaminaCostModifier = 1.1f;
    public float heavyAttackStaminaCostModifier = 1.4f;
    public float chargeAttackStaminaCostModifier = 1.8f;
    public float lightRunAttackStaminaCostModifier = 1.3f;
    public float lightRollAttackStaminaCostModifier = 1.0f;
    public float lightBackStepAttackStaminaCostModifier = 0.6f;

    //item based actions
    [Header("Item Based Actions")]
    public WeaponItemAction oh_r1_Action; //one handed r1 weapon action 
    public WeaponItemAction oh_r2_Action; // one handed r2 heavy attack action 

    //special abilities

    //blocking 

    [Header("Whooshes")]
    public AudioClip[] whooshes;


}
