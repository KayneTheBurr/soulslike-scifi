using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effect/Take Stamina Damage")]

public class TakeStaminaDamageEffect : InstantCharacterEffect
{
    public float staminaDamage;

    public override void ProcessEffect(CharacterManager character)
    {
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        if(character.IsOwner)
        {
            character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            Debug.Log("Character takes" + staminaDamage + "stamina damage");

        }
    }
}
