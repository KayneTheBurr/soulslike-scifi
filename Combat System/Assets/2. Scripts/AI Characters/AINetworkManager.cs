using Unity.Netcode;
using UnityEngine;

public class AINetworkManager : CharacterNetworkManager
{
    AICharacterManager aiCharacter;

    


    protected override void Awake()
    {
        base.Awake(); 
        aiCharacter = GetComponent<AICharacterManager>();
    }
}
