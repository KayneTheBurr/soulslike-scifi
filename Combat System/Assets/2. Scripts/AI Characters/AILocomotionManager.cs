using UnityEngine;

public class AILocomotionManager : CharacterLocomotionManager
{
    AICharacterManager aiCharacter;



    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if(aiCharacter.aiNetworkManager.isMoving.Value) //if the ai character is moving do this 
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }
    
}
