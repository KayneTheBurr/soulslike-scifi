using UnityEngine;


[CreateAssetMenu(menuName = "AI / States / Attack ")]
public class AttackState : AIStates
{
    [Header("Current Attack")]
    [HideInInspector] public AICharacterAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;

    [Header("State Flags")]
    protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;

    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;


    public override AIStates Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCombatManager.currentTarget == null) // go to idle if target is null 
            return SwitchState(aiCharacter, aiCharacter.idle);

        if (aiCharacter.aiCombatManager.currentTarget.isDead.Value) // go to idle if target is dead
            return SwitchState(aiCharacter, aiCharacter.idle);

        aiCharacter.aiCombatManager.RotateTowardsTargetWhileAttacking(aiCharacter);

        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);

        //perform a combo 
        if(willPerformCombo && !hasPerformedCombo)
        {
            //
            if(currentAttack.comboAction != null)
            {
                //if can combo 
                //hasPerformedCombo = true;
                //currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
            }
        }

        if (aiCharacter.isPerformingAction) return this;

        //
        if (!hasPerformedAttack)
        {
            if (aiCharacter.aiCombatManager.actionRecoveryTimer > 0) return this;
            
            PerformAttack(aiCharacter);

            //return to top so that we can combo if we are able to 
            return this;
        }

        if(pivotAfterAttack)
            aiCharacter.aiCombatManager.PivotTowardsTarget(aiCharacter);
        
        return SwitchState(aiCharacter, aiCharacter.combatStance);

    }
    protected void PerformAttack(AICharacterManager aiCharacter)
    {
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(aiCharacter);

        //set the recovery timer to the time associated with its current attack value 
        aiCharacter.aiCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        hasPerformedAttack = false;
        hasPerformedCombo = false;

    }
}
