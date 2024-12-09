using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [HideInInspector] public AICombatManager aiCombatManager;
    [HideInInspector] public AINetworkManager aiNetworkManager;
    [HideInInspector] public AILocomotionManager aiLocomotionManager;

    [Header("Character Name")]
    public string characterName = "";

    [Header("NavMesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("Current State")]
    [SerializeField] AIStates currentState;

    [Header("States")]
    public IdleState idle;
    public PursueTargetState pursueTarget;
    public CombatStanceState combatStance;
    public AttackState attack;

    [Header("Temp Testing")]
    public int maxHP;
    

    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        aiCombatManager = GetComponent<AICombatManager>();
        aiNetworkManager = GetComponent<AINetworkManager>();
        aiLocomotionManager = GetComponent<AILocomotionManager>();

        idle = Instantiate(idle);
        pursueTarget = Instantiate(pursueTarget);

        currentState = idle;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsOwner)
        {
            //force setting network variable HP on spawn
            aiNetworkManager.maxHealth.Value = maxHP;
            aiNetworkManager.currentHealth.Value = maxHP;

            idle = Instantiate(idle);
            pursueTarget = Instantiate(pursueTarget);
            combatStance = Instantiate(combatStance);
            attack = Instantiate(attack);
            currentState = idle;
        }

        aiNetworkManager.currentHealth.OnValueChanged += aiNetworkManager.CheckHP;

    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        aiNetworkManager.currentHealth.OnValueChanged -= aiNetworkManager.CheckHP;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if(characterUIManager.hasFloatingHPBar)
            characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHPChanged;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (characterUIManager.hasFloatingHPBar)
            characterNetworkManager.currentHealth.OnValueChanged -= characterUIManager.OnHPChanged;
    }

    private void ProcessStateMachine()
    {
        AIStates nextState = currentState?.Tick(this);

        if (nextState != null)
        {
            currentState = nextState;
        }

        //reset the position/rotation after teh state machine has processed it
        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;

        if (aiCombatManager.currentTarget != null)
        {
            aiCombatManager.distanceFromTarget = Vector3.Distance(aiCombatManager.currentTarget.transform.position, transform.position);
            aiCombatManager.targetDirection = aiCombatManager.currentTarget.transform.position - transform.position;
            aiCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCombatManager.targetDirection);
        }


        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if(remainingDistance > navMeshAgent.stoppingDistance)
            {
                aiNetworkManager.isMoving.Value = true;
            }
            else
            {
                aiNetworkManager.isMoving.Value = false;
            }
        }
        else
        {
            aiNetworkManager.isMoving.Value = false;
        }


    }

    protected override void Update()
    {
        base.Update();
        aiCombatManager.HandleActionRecovery(this);

    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(IsOwner)
        {
            ProcessStateMachine();
        }
        

    }
}
