using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.InputSystem.Processors;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead =
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    

    [Header("Flags")]
    public bool isPerformingAction;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;
    public bool isSprinting = false; //add to network

    
    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
        animator.SetBool("isGrounded", isGrounded);

        if(IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position,
                characterNetworkManager.networkPosition.Value,
                ref characterNetworkManager.networkPositionVelocity,
                characterNetworkManager.networkPositionSmoothTime);

            transform.rotation = Quaternion.Slerp(transform.rotation,
                characterNetworkManager.networkRotation.Value,
                characterNetworkManager.networkRotationSmoothTime);
        }
    }
    protected virtual void LateUpdate()
    {

    }

    public virtual IEnumerator HandleDeathEvents(bool manuallySelectDeathAnim = false)
    {
        if(IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            //reset all flags that need to be reset

            //if we are not grounded, play arial death animation
            if(!manuallySelectDeathAnim)
            {
                characterAnimatorManager.PlayTargetActionAnimation("SS_Main_Dead_01", true);
            }
            //play death vfx/sfx

            yield return new WaitForSeconds(5);

            //award players some currency for slaying enemy 

            //disable the character
        }
    }

    public virtual void ReviveCharacter()
    {



    }




}
