using TMPro;
using UnityEngine;

public class UI_Character_HP_Bar : UI_StatBar
{
    //Same as our own health bar, but this bar appears/disappears in world space above a character when they are hit
    //always face the camera

    private CharacterManager character;
    private AICharacterManager aiCharacter;
    private PlayerManager player;

    [SerializeField] GameObject characterUICanvas;
    [SerializeField] bool displayCharacterNameOnDamage = false;
    [SerializeField] float defaultTimeBeforeBarHides = 3;
    [SerializeField] float hideTimer = 0;
    [SerializeField] float currentDamageTaken = 0;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterDamage;
    [HideInInspector] public float oldHPValue = 0;

    protected override void Awake()
    {
        base.Awake();
        character = GetComponentInParent<CharacterManager>();

        if(character != null )
        {
            aiCharacter = character as AICharacterManager;
            player = character as PlayerManager;
        }

    }

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        characterUICanvas.transform.LookAt(PlayerCamera.instance.transform.position + transform.position);

        if(hideTimer > 0 )
        {
            hideTimer -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        currentDamageTaken = 0;
    }
    public override void SetStat(float newValue)
    {
        if(displayCharacterNameOnDamage)
        {
            characterName.enabled = true;
            
            if(aiCharacter != null)
            {
                characterName.text = aiCharacter.characterName;
            }
            if(player != null)
            {
                characterName.text = player.playerNetworkManager.characterName.Value.ToString();
            }   
        }

        //call this here in case max health has changed from an effect/buff etc
        slider.maxValue = character.characterNetworkManager.maxHealth.Value;

        //bar that drops slower in background/health to regain bb style maybe

        //total damage taken while the bar is active
        
        currentDamageTaken = Mathf.RoundToInt(currentDamageTaken + (oldHPValue - newValue));

        if (currentDamageTaken < 0)
        {
            currentDamageTaken = Mathf.Abs(currentDamageTaken);
            characterDamage.text = "+ " + currentDamageTaken;
        }
        else
        {
            characterDamage.text = "- " + currentDamageTaken;
        }
        slider.value = newValue;

        if(character.characterNetworkManager.currentHealth.Value != character.characterNetworkManager.maxHealth.Value)
        {
            hideTimer = defaultTimeBeforeBarHides;
            gameObject.SetActive(true);
        }
    }
}
