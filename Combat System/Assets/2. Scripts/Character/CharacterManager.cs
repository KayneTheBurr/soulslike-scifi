using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    [Header("Flags")]
    public bool isPerformingAction;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;
    public bool isSprinting = false;


    [Header("Stats")]
    public int endurance = 10;
    public int currentStamina = 1;


    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {

    }
    protected virtual void LateUpdate()
    {

    }
}
