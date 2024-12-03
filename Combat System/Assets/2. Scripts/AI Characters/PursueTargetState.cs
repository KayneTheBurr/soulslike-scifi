using UnityEngine;
using UnityEngine.AI;


[CreateAssetMenu(menuName = "AI / States / Pursue")]
public class PursueTargetState : AIStates
{



    public override AIStates Tick(AICharacterManager aiCharacter)
    {
        
        //check if we are performing an action(if so do nothing until the action is done)
        if (aiCharacter.isPerformingAction) return this;

        //check if our target is null, if we do not have a target, return to idle state
        if (aiCharacter.aiCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idle);

        //make sure navmeshagent is active, if not enable it 
        if(aiCharacter.navMeshAgent.enabled == false)
        {
            aiCharacter.navMeshAgent.enabled = true;
        }
        aiCharacter.aiLocomotionManager.RotateTowardsAgent(aiCharacter);
        //if within range of target, switch to combat state

        //if target is not reachable and they are far away, return back to "home" area 

        //pursue the target
        // option 1
        //aiCharacter.navMeshAgent.SetDestination(aiCharacter.aiCombatManager.currentTarget.transform.position);

        // option 2
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);
        return this;
    }
}
