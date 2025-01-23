using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class AIBossCharacterManager : AICharacterManager
{
    //give this ai a unique ID
    public int bossID = 0;
    [SerializeField] bool hasBeenDefeated = false;


    //if the save file does not contain a boss with this ID, add it 
    //if it is present, check if the boss ahs been defeated or not 
    //if the boss has been defeated, disable this 
    //if the boss has NOT been defeated, allow this object to continue to be active 

    //when this AI is spawned, check save file (dictionary)
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsServer) //this is the host then 
        {
            //if out save data does not contain info for this boss, add it now
            if(WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
            }
            //otherwise load the data we have on the boss
            else
            {
                hasBeenDefeated = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];

                if(hasBeenDefeated)
                {
                    gameObject.SetActive(false);
                }
                else
                {

                }
            }
        }
    }


}
