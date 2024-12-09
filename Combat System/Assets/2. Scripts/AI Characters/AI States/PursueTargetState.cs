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

        //if our target is outside of our FOV, pivot to face them 
        if(aiCharacter.aiCombatManager.viewableAngle < aiCharacter.aiCombatManager.minFOV ||
            aiCharacter.aiCombatManager.viewableAngle > aiCharacter.aiCombatManager.minFOV)
        {
            aiCharacter.aiCombatManager.PivotTowardsTarget(aiCharacter);
        }


        aiCharacter.aiLocomotionManager.RotateTowardsAgent(aiCharacter);

        //if within range of target, switch to combat state
        //option 1 (better for ranged enemies or melee/ranged hybrid enemies)
        //if(aiCharacter.aiCombatManager.distanceFromTarget <= aiCharacter.combatStance.maxEngagementDistance)
        //    return SwitchState(aiCharacter, aiCharacter.combatStance);

        //option 2 (melee enemies who will try to close the gap more)
        if (aiCharacter.aiCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            return SwitchState(aiCharacter, aiCharacter.combatStance);

        //if target is not reachable and they are far away, return back to "home" area 

        //pursue the target
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);
        return this;
    }
}
