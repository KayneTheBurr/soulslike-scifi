using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;



    [Header("Ground Checks And Jumping")]
    [SerializeField] float groundCheckSphereRadius = 1f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] protected float gravityForce = -9.81f;
    
    [SerializeField] protected Vector3 yVelocity; //measure the force characters are pulled downward
    [SerializeField] protected float groundedYVelocity = -20; //sticks us to the ground while grounded
    [SerializeField] protected float fallStartYVelocity = -5; //down force at the START of our fall, rises over time 
    protected bool fallingVelocitySet = false;
    protected float inAirTimer = 0;

    [Header("Flags")]
    public bool isRolling = false;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    protected virtual void Update()
    {
        if(HandleGroundChecks())//if we are grounded do this
        {
            //if we are not attempting to jump or are falling
            if(yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocitySet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else //if we are not grounded do this 
        {
            if(!character.characterNetworkManager.isJumping.Value && !fallingVelocitySet) //if not jumping AND falling speed not set
            {
                fallingVelocitySet = true;
                yVelocity.y = fallStartYVelocity;
            }
            inAirTimer += Time.deltaTime;
            character.animator.SetFloat("InAirTimer", inAirTimer);
            yVelocity.y += gravityForce * Time.deltaTime;
        }
        //always apply force downward on the player regardless of grounded or not 
        character.characterController.Move(yVelocity * Time.deltaTime);
    }

    public void EnableCanRotate()
    {
        character.canRotate = true;
    }

    public void DisableCanRotate()
    {
        character.canRotate = false;
    }

    protected virtual bool HandleGroundChecks()
    {
        character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
        return character.isGrounded;
    }

    //draw the ground sphere for grounded checks 
    protected void OnDrawGizmosSelected()
    {
        //Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
    }

}
