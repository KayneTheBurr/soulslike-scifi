using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

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

    






}
