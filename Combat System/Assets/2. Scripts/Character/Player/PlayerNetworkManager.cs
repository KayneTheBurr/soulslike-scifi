using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerNetworkManager : CharacterNetworkManager
{
    PlayerManager player;

    public NetworkVariable<FixedString64Bytes> characterName = 
        new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }



    public void SetNewMaxHealthValue( int oldValue, int newValue)
    {
        maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitality(newValue);
        PlayerUIManager.instance.playerHUDManager.SetMaxHealthValue(maxHealth.Value);
        currentHealth.Value = maxHealth.Value;
    }

    public void SetNewMaxStaminaValue(int oldValue, int newValue)
    {
        maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEndurance(newValue);
        PlayerUIManager.instance.playerHUDManager.SetMaxStaminaValue(maxStamina.Value);
        currentStamina.Value = maxStamina.Value;
    }

}
