using UnityEngine;

public class AIStates : ScriptableObject
{
    public virtual AIStates Tick(AICharacterManager aiCharacter)
    {
       

        return this;
    }
    public virtual AIStates SwitchState(AICharacterManager aiCharacter, AIStates newState)
    {
        ResetStateFlags(aiCharacter);
        return newState;
    }
    protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
    {

    }
}
