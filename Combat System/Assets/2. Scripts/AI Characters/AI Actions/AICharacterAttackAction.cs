using UnityEngine;


[CreateAssetMenu(menuName = "AI / Actions / Attack ")]
public class AICharacterAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] private string attackAnimation;

    [Header("Combo Actions")]
    public AICharacterAttackAction comboAction; //the combo action of this attack action

    [Header("Action Values")]
    public int attackWeight;
    [SerializeField] AttackType attackType;
    //attack can be repeated
    public float actionRecoveryTime = 1.5f;
    public float minAttackAngle = -35;
    public float maxAttackAngle = 35;
    public float minAttackDistance = 0;
    public float maxAttackDistance = 3;


    public void AttemptToPerformAction(AICharacterManager aiCharacter)
    {
        aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType, attackAnimation, true);
    }
}
