using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerManager : CharacterManager
{
    [Header("Debug Menu")]
    [SerializeField] public bool respawnCharacter = false;
    [SerializeField] public bool switchRightWeapon = false;

    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;

    protected override void Awake()
    {
        //do character manager awake stuff 
        base.Awake();

        //after doing charcter stuff then player only stuff gets done next 
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
    }

    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(gameObject);
    }
    protected override void Update()
    {
        base.Update();
        if (!IsOwner) return; //only do things if you are the owner of the player

        playerLocomotionManager.AllMovement();
        playerStatsManager.RegenerateStamina();

        DebugMenu();
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraAction();

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        
    }

    public override IEnumerator HandleDeathEvents(bool manuallySelectDeathAnim = false)
    {
        if(IsOwner)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
        }
        return base.HandleDeathEvents(manuallySelectDeathAnim);
        //check fpr players that are alive, respawn if none 
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

        if(IsOwner)
        {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
            WorldSaveGameManager.instance.player = this;

            //playerNetworkManager.endurance.Value = 10;

            //update stat when its corresponding attribute is chagned 
            playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;


            //read stat value, set stat any time it is changed
            
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewStaminaValue;
            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewHealthValue;

        }
        //only update floating HP bar if we arent the owner, only want to see other players bars not our own 
        if (!IsOwner)
            characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;

        //Stats
        playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

        //Equipment
        playerNetworkManager.currentRightWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

        //lock on 
        playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockOnChanged;
        playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange;

        //Flags
        playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChanged;

        //if we connect to someone elses world, reload our character data to this new character
        //dont run this if we are the server host 
        if (IsOwner && !IsServer)
        {
            LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
        }


    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

        if (IsOwner)
        {
            //update stat when its corresponding attribute is chagned 
            playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;

            //read stat value, set stat any time it is changed
            playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
            playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerHUDManager.SetNewStaminaValue;
            playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerHUDManager.SetNewHealthValue;
        }
        if (!IsOwner)
            characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
        //Stats
        playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;

        //Equipment
        playerNetworkManager.currentRightWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

        //lock on 
        playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockOnChanged;
        playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;

        //Flags
        playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChanged;

    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this); 

        //if we are the host, we dont need to sync other players, only if joining the game late
        if(!IsServer && IsOwner)
        {
            foreach(var player in WorldGameSessionManager.instance.players)
            {
                if(player != this)
                {
                    player.LoadOtherPlayerCharactersWhenJoiningServer();
                }
            }
        }
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
        currentCharacterData.xPos = transform.position.x;
        currentCharacterData.yPos = transform.position.y;
        currentCharacterData.zPos = transform.position.z;
        currentCharacterData.sceneIndexNumber = SceneManager.GetActiveScene().buildIndex;

        currentCharacterData.vitality = playerNetworkManager.vitality.Value;
        currentCharacterData.endurance = playerNetworkManager.endurance.Value;
        currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;
        currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;


    }

    public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        playerNetworkManager.characterName.Value = currentCharacterData.characterName;
        Vector3 myPos = new Vector3(currentCharacterData.xPos, currentCharacterData.yPos, currentCharacterData.zPos);
        transform.position = myPos;

        playerNetworkManager.vitality.Value = currentCharacterData.vitality;
        playerNetworkManager.endurance.Value = currentCharacterData.endurance;

        playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitality(currentCharacterData.vitality);
        playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEndurance(currentCharacterData.endurance);
        playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
        playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
        PlayerUIManager.instance.playerHUDManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if(IsOwner)
        {
            playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
            playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
            isDead.Value = false;
            //restore flux to max value
            
            //play rebirth effects
            playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
            
        }
    }

    public void LoadOtherPlayerCharactersWhenJoiningServer()
    {
        //sync weapons 
        playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightWeaponID.Value);
        playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftWeaponID.Value);

        //lock on target
        if(playerNetworkManager.isLockedOn.Value)
        {
            playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
        }
        //armor
        //consmetic choices

    }

    //Debug for testing
    private void DebugMenu()
    {
        if(respawnCharacter)
        {
            respawnCharacter = false;
            ReviveCharacter();
        }
        if(switchRightWeapon)
        {
            switchRightWeapon = false;
            playerEquipmentManager.SwitchRightWeapon();

        }
    }

}
