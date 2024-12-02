using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [HideInInspector] public AICombatManager aiCombatManager;
    [HideInInspector] public AINetworkManager aiNetworkManager;

    [Header("NavMesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("Current State")]
    [SerializeField] AIStates currentState;

    [Header("States")]
    public IdleState idle;
    public PursueTargetState pursueTarget;
    //combat
    //attack

    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        aiCombatManager = GetComponent<AICombatManager>();
        aiNetworkManager = GetComponent<AINetworkManager>();

        idle = Instantiate(idle);
        pursueTarget = Instantiate(pursueTarget);

        currentState = idle;
    }
    private void ProcessStateMachine()
    {
        AIStates nextState = currentState?.Tick(this);

        if(nextState != null)
        {
            currentState = nextState;
        }
        if(navMeshAgent.enabled)
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
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        ProcessStateMachine();

    }
}
