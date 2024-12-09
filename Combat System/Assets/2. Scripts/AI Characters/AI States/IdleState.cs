using UnityEngine;

[CreateAssetMenu(menuName = "AI / States / Idle")]
public class IdleState : AIStates
{


    public override AIStates Tick(AICharacterManager aiCharacter)
    {
        
        if(aiCharacter.characterCombatManager.currentTarget != null)
        {
            //return the pursue target state instead

            return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            
        }
        else
        {
            //return the same (idle) state, keep searching for a target
            aiCharacter.aiCombatManager.FindATargetViaLineOfSight(aiCharacter);
            return this;
        }
        
    }


}
