using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.Processors;
using NUnit.Framework;

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
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterSFXManager characterSFXManager;
    [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
    [HideInInspector] public CharacterUIManager characterUIManager;

    [Header("Character Group")]
    public CharacterGroup characterGroup;

    [Header("Flags")]
    public bool isPerformingAction;
    public bool isGrounded = true;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;
    
    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterSFXManager = GetComponent<CharacterSFXManager>();
        characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
        characterUIManager = GetComponent<CharacterUIManager>();
    }
    protected virtual void Start()
    {
        IgnoreMyOwnColliders();
    }
    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        animator.SetBool("IsMoving", characterNetworkManager.isMoving.Value);
        characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
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
    protected virtual void FixedUpdate()
    {

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

    protected virtual void IgnoreMyOwnColliders()
    {
        //get all the colliders on us(character controller) and children (damagable bone colliders)
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damagableCharacterColliders = GetComponentsInChildren<Collider>();

        //make a list of colliders to ignore and add all owners own colliders to the list to ignore
        List<Collider> collidersToIgnore = new List<Collider>();
        foreach(var collider in damagableCharacterColliders)
        {
            collidersToIgnore.Add(collider);
        }
        collidersToIgnore.Add(characterControllerCollider);

        //go through every collider on the list and make them ignore one another
        foreach(var collider in collidersToIgnore)
        {
            foreach(var otherCollider in collidersToIgnore)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }

    }


}
