using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI / States / Combat Stance ")]
public class CombatStanceState : AIStates
{
    //1. Select an attack for the attack state based on: DIstance, Angle, Chance/weight, probability etc
    //2. process combat logic while waiting to attack(dodge/block/strafe etc)
    //3. if target moves out of range, return to pursue state to chase
    //4. if target is too far away/no longer there, switch to idle state

    [Header("Attacks")]
    public List<AICharacterAttackAction> aiCharacterAttacks; //list of all possible attacks the character CAN do
    private List<AICharacterAttackAction> potentialAttacks; //all attacks possible in the situation based on angle/distance
    [SerializeField] private AICharacterAttackAction chosenAttack;
    [SerializeField] private AICharacterAttackAction previousAttack;
    protected bool hasAttacked = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;
    [SerializeField] protected int chanceToPerfromCombo = 25; //0-100 for if enemy will perform combo attack 
    protected bool hasRolledForComboChance = false;

    [Header("Engagement Distance")]
    public float maxEngagementDistance = 5; //The distance we have to be away from target before returning to pursue state

    public override AIStates Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction) return this;

        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true; //turn on nev mesh if it was off 

        //get the ai character to turn and face the target when its outisde its FOV
        if(!aiCharacter.aiNetworkManager.isMoving.Value)
        {
            if(aiCharacter.aiCombatManager.viewableAngle < -35 || aiCharacter.aiCombatManager.viewableAngle < 35 )
            {
                aiCharacter.aiCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }

        //rotate to face our target
        aiCharacter.aiCombatManager.RotateTowardsAgent(aiCharacter);

        //if our target is no longer there, switch back to idle 
        if (aiCharacter.aiCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idle);

        if(!hasAttacked)
        {
            GetNewAttack(aiCharacter);
        }
        else
        {
            aiCharacter.attack.currentAttack = chosenAttack;

            //roll for combo chance or other outcome
            
            return SwitchState(aiCharacter, aiCharacter.attack);
        }

        //if we get outside of the combat state range, return to pursue state
        if(aiCharacter.aiCombatManager.distanceFromTarget > maxEngagementDistance)
            return SwitchState(aiCharacter, aiCharacter.pursueTarget);

        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);
        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter)
    {
        potentialAttacks = new List<AICharacterAttackAction>();
        foreach(var attack in aiCharacterAttacks)
        {
            //if we are too close, then continue onto the next attack choice
            if (attack.minAttackDistance > aiCharacter.aiCombatManager.distanceFromTarget) continue;

            //if we are too far away, then continue onto the next attack choice
            if(attack.maxAttackDistance < aiCharacter.aiCombatManager.distanceFromTarget) continue;

            //check the attack angle, if outside FOV for the attack check the next attack
            if (attack.minAttackAngle > aiCharacter.aiCombatManager.distanceFromTarget) continue;
            if (attack.maxAttackAngle < aiCharacter.aiCombatManager.distanceFromTarget) continue;

            potentialAttacks.Add(attack);
        }
        if(potentialAttacks.Count <= 0)
        {
            Debug.Log("no attack option");
            return;
        }
        var totalWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            totalWeight += attack.attackWeight;
        }
        var randomWeight = Random.Range(1, totalWeight + 1);
        var processedWeight = 0;

        foreach(var attack in potentialAttacks)
        {
            processedWeight += attack.attackWeight;
            if(randomWeight <= processedWeight)
            {
                chosenAttack = attack;
                previousAttack = chosenAttack;
                hasAttacked = true;
                return;
            }
        }
        
        // pick one of the remaining attacks randomly form list 
    }

    protected virtual bool RollForOutcomeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;
        int randomPercentage = Random.Range(0, 100);
        if(randomPercentage < outcomeChance)
        {
            outcomeWillBePerformed = true;
        }
        return outcomeWillBePerformed;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasRolledForComboChance = false;
        hasAttacked = false;

    }



}
