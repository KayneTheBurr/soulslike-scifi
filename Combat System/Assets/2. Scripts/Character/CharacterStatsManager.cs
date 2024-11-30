using System.Globalization;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;




    [Header("Stamina Regeneration")]
    public float staminaRegenTimer = 0;
    public float staminaRegenDelay = 2;
    public float staminaTickTimer = 0;
    public float staminaTickRate = 0.1f;
    public float staminaRegenAmount = 5;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {

    }

    public int CalculateHealthBasedOnVitality(int vitality)
    {
        float health = 0;

        health = vitality * 15;

        return Mathf.RoundToInt(health);

    }

    public int CalculateStaminaBasedOnEndurance(int endurance)
    {
        float stamina = 0;

        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
        
    }
    public virtual void RegenerateStamina()
    {
        if (!character.IsOwner) return;

        if (character.isSprinting) return;

        if (character.isPerformingAction) return;

        staminaRegenTimer += Time.deltaTime;
        if (staminaRegenTimer >= staminaRegenDelay)
        {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= staminaTickRate)
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegenAmount;
                }

            }
        }


    }

    public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
    {
        //reset stamina regen timer only if we used stamina -> stamina went down
        //dont reset if stamina is already going up 
        if(newValue < oldValue)
        {
            staminaRegenTimer = 0;
        }

    }
}
