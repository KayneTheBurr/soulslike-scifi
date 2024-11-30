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

    //stat scaling
    //weapon modifiers
    //light/heavy attack modifiers
    //critical modifiers

    [Header("Weapon Base Poise Damage")]
    public float poiseDamage = 10;
    //weapon hyperarmor?

    //weapon blocking absorbtion and stagger

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;
    //running attack cost
    //light modifier
    //heavy modifier

    //item based actions

    //special abilities

    //blocking 
}
