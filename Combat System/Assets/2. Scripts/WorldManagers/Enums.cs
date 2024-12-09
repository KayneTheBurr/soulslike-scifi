using UnityEngine;




public class Enums : MonoBehaviour
{
    
}
public enum CharacterSlot
{
    CharacterSlot_01, CharacterSlot_02, CharacterSlot_03, CharacterSlot_04, CharacterSlot_05, 
    CharacterSlot_06, CharacterSlot_07, CharacterSlot_08, CharacterSlot_09, CharacterSlot_10, No_Slot
}
public enum WeaponModelSlot
{
    RightHand,
    LeftHand//,
    //RightHip,
    //LeftHip,
    //Back
}
//used to determine damage based on attack type
public enum AttackType
{
    LightAttack01,
    LightAttack02, 
    HeavyAttack01,
    HeavyAttack02,
    ChargeAttack01, 
    ChargeAttack02,
    LightRunningAttack01,
    LightRollingAttack01,
    LightBackStepAttack01

}

public enum CharacterGroup
{
    Friendly, 
    Enemy
}

