using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical, horizontal;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");

    }
    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
    {
        float horizontalAmt = horizontalValue;
        float verticalAmt = verticalValue;

        if(isSprinting )
        {
            verticalAmt = 2;
        }
        character.animator.SetFloat(horizontal, horizontalAmt, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, verticalAmt, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, 
                            bool isPerformingAction, bool applyRootMotion = true,
                            bool canRotate = false, bool canMove = false)
    {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);

        //stop character from performing other actions
        character.isPerformingAction = isPerformingAction;
        character.canMove = canMove;
        character.canRotate = canRotate;





    }



}
