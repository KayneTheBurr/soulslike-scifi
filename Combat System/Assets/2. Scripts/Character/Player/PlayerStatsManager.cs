using UnityEngine;
using UnityEngine.Events;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;
    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();

    }

    protected override void Start()
    {
        base.Start();

        //why calc here? when we spawn the character in need to calculate the stats iniitally until the 
        CalculateHealthBasedOnVitality(player.playerNetworkManager.vitality.Value);
        CalculateStaminaBasedOnEndurance(player.playerNetworkManager.endurance.Value);

    }



}
