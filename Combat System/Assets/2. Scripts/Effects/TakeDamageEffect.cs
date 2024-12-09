using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effect/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage; //if dmg is done by another character attack, store who they are here 

    [Header("Damage")]
    public float physicalDamage = 0; //break down into sub types (standard, slash, pierce, strike)
    public float plasmaDamage = 0;
    public float electricalDamage = 0;
    public float cryoDamage = 0;
    public float chemicalDamage = 0;
    public float geneticDamage = 0;
    public float radiationDamage = 0;

    //build up effects added as well here 

    [Header("Final Damage")] //damage character actually takes after all effects have been processed
    private float finalDamage = 0;

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false; //false by defualt, play "stun" animation if poise is actually broken by effect

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound Effects")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementalDamageSFX; //additive sfx if damage type has an element attached to it additionally

    [Header("Direction Damage Taken From")]
    //determine where hit from for which animation to play, where blood sprays etc 
    public float angleHitFrom;
    public Vector3 contactPoint; //where blood fx plays from 

    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);

        // if the character is dead, dont process any additional damage effects since they are already dead
        if (character.isDead.Value) return;

        //check for invulnerability window
        if (character.characterNetworkManager.isInvulnerable.Value) return;

        CalculateDamage(character);

        //check which direction damage came from and play damage animation
        PlayDirectionalBasedDamageAnimation(character);

        //check for build up effect (poison bleed etc)

        //play damage sound fx
        PlayDamageSFX(character);

        //play damge vfx (blood etc)
        PlayDamageVFX(character);

        //if characcter is AI enemey, check for who just attacked them 

    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;
        
        if (characterCausingDamage != null) //if damage is doen by another character and not the environment
        {
            //check the characters modifiers and modify the base damage

        }
        //check character for flat damage reduction, subtract them from the specific type of damage

        //check for character armor absorbtions, subtract percentage from damage

        //add all damage types together and apply final damage
        finalDamage = (physicalDamage + plasmaDamage + electricalDamage + chemicalDamage + cryoDamage + geneticDamage + radiationDamage);
        if(finalDamage <= 0)
        {
            finalDamage = 1;
        }

        Debug.Log("Dealt " + finalDamage + " damage!");

        character.characterNetworkManager.currentHealth.Value -= finalDamage;
        //calculate poise damage to determine if character will be stunned and play damaged animation or not 
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        //play a special effect based on element type
        

        character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
    }
    private void PlayDamageSFX(CharacterManager character)
    {
        
        AudioClip physicalDamageSFX = WorldSFXManager.instance.ChooseRandomSFXFromArray(WorldSFXManager.instance.physicalDamageSFX);

        character.characterSFXManager.PlaySoundFX(physicalDamageSFX, 0.5f);
        character.characterSFXManager.PlayDamageGrunt();
        //play more sfx based on damage type done
    }

    private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
    {
        
        if (!character.IsOwner) return;
        if (character.isDead.Value) return;

        //Calculate if poise is broken
        poiseIsBroken = true;

        if(angleHitFrom >= 145 &&  angleHitFrom <= 180)
        {
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.Fwd_Med_Damage);
            
        }
        else if (angleHitFrom >= -180 && angleHitFrom <= -145)
        {
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.Fwd_Med_Damage);
        }
        else if (angleHitFrom >= 45 && angleHitFrom <= 144)
        {
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.Right_Med_Damage);
            
        }
        else if (angleHitFrom >= -144 && angleHitFrom <= -45)
        {
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.Left_Med_Damage);
            
        }
        else if (angleHitFrom >= -45 && angleHitFrom <= 45)
        {
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.Back_Med_Damage);
            
        }
        
        // if poise is broken, play the stagger animation
        if(poiseIsBroken)
        {
            character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
            character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }

    }

}   

