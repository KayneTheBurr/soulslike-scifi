using UnityEngine;

public class AICombatManager : CharacterCombatManager
{
    protected AICharacterManager aiCharacter;

    [Header("Action Recovery")]
    public float actionRecoveryTimer = 0;

    [Header("Target Info")]
    public float viewableAngle;
    public float distanceFromTarget;
    public Vector3 targetDirection;

    [Header("Detection")]
    [SerializeField] float detectionRadius = 10;
    public float minFOV = -35;
    public float maxFOV = 35;

    [Header("Attack Rotation Speed")]
    public float attackTrackingSpeed = 20;


    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
    }
    public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if (currentTarget != null) return;

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length ; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            if(targetCharacter == null) continue;
            if (targetCharacter == aiCharacter) continue;
            if (targetCharacter.isDead.Value) continue;

            if (WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
            {
                //if a potential target is found, is it in front of us?
                Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                if(angleOfPotentialTarget > minFOV && angleOfPotentialTarget < maxFOV)
                {
                    Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position,
                        targetCharacter.characterCombatManager.lockOnTransform.position, Color.green);
                    //last, check for linecast if the target is obscured
                    if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position, 
                            targetCharacter.characterCombatManager.lockOnTransform.position,
                            WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position,
                            targetCharacter.characterCombatManager.lockOnTransform.position, Color.red);
                    }
                    else
                    {
                        targetsDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetsDirection);

                        //assign the target
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                        //Once target is found, turn/pivot towards the target rather than slowly walking at an angle towards them 
                        PivotTowardsTarget(aiCharacter);
                    }
                }
            }
        } 
    }

    public void PivotTowardsTarget(AICharacterManager aICharacter)
    {
        //play a pivot animation depending on viewabe angle of targett character
        if (aICharacter.isPerformingAction) return;

        if(viewableAngle >= 30 && viewableAngle <= 60)
        {
            aICharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_R45_01", true);
        }
        else if (viewableAngle <= -30 && viewableAngle >= -60)
        {
            aICharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_L45_01", true);
        }
        else if (viewableAngle > 60 && viewableAngle <= 110)
        {
            aICharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_R90_01", true);
        }
        else if (viewableAngle < -60 && viewableAngle >= -110)
        {
            aICharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_L90_01", true);
        }
        else if (viewableAngle > 110 && viewableAngle <= 145)
        {
            aICharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_R135_01", true);
        }
        else if (viewableAngle < -110 && viewableAngle >= -145)
        {
            aICharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_L135_01", true);
        }
        else if (viewableAngle > 145 && viewableAngle <= 180)
        {
            aICharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_R180_01", true);
        }
        else if (viewableAngle < -145 && viewableAngle > -180)
        {
            aICharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_L180_01", true);
        }
    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if(aiCharacter.aiNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }

    public void RotateTowardsTargetWhileAttacking(AICharacterManager aiCharacter)
    {
        if (currentTarget == null) return;

        //check if we can rotate 
        if (aiCharacter.canRotate) return;

        if (aiCharacter.isPerformingAction) return;

        //rotate towards the target at a specified rotation speed during specific frames
        Vector3 targetDirection = currentTarget.transform.position - transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero)
            targetDirection = aiCharacter.transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackTrackingSpeed);
    }

    public void HandleActionRecovery(AICharacterManager aICharacter)
    {
        if(actionRecoveryTimer > 0)
        {
            if(!aICharacter.isPerformingAction)
            {
                actionRecoveryTimer -= Time.deltaTime;
            }
        }
    }
            

}
