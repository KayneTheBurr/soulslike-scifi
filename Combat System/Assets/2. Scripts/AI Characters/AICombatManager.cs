using UnityEngine;

public class AICombatManager : CharacterCombatManager
{
    [Header("Detection")]
    [SerializeField] float detectionRadius = 10;
    [SerializeField] float minDetectionAngle = -35;
    [SerializeField] float maxDetectionAngle = 35;


    public void FindATargetVioLineOfSight(AICharacterManager aiCharacter)
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
                float viewableAngle = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                if(viewableAngle > minDetectionAngle && viewableAngle < maxDetectionAngle)
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
                        Debug.Log("Blocked");
                    }
                    else
                    {
                        //assign the target
                        //update this with "SET TARGET" function once lock on system is finished to update lock target with the network 
                        aiCharacter.characterCombatManager.currentTarget = targetCharacter;
                    }


                }


            }


        }
        
    }

}
