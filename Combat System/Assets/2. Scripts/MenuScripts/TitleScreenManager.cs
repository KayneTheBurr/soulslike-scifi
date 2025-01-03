using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    //testing network function attempt 23580620358762
    [SerializeField] private bool isHostInstance;

    [Header("Menus")]
    public GameObject mainMenu, loadCharactersMenu;

    [Header("Buttons")]
    public Button loadCharacterMenuReturnButton;
    public Button mainMenuLoadGameButton;
    public Button mainMenuNewGameButton;

    [Header("Pop-ups")]
    [SerializeField] GameObject noCharacterSlotsFreePopUp;
    [SerializeField] Button noCharacterSlotsFreeOkayButton;
    [SerializeField] GameObject deleteCharacterSlotPopUp;
    [SerializeField] Button deleteCharacterSlotConfirmButton;

    [Header("Save Slots")]
    public CharacterSlot currentSelectedCharacterSlot = CharacterSlot.No_Slot;
    

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartNetworkAsHost()
    {
        //original single line here before debuggin unity 6 shit
        //NetworkManager.Singleton.StartHost();

        //Debug.Log("Configuring port for Host...");

        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();

        // Set port depending on whether this is the main host or the clone
        if (isHostInstance) // Main host
        {
            transport.ConnectionData.Port = 7778;
            //Debug.Log("Starting Main Host on port 7778...");
        }
        else // Clone host
        {
            transport.ConnectionData.Port = 7780;
            //Debug.Log("Starting Clone Host on port 7780...");
        }

        if (!NetworkManager.Singleton.StartHost())
        {
            Debug.LogError("Failed to start Host! Check port conflicts or transport configuration.");
        }
        else
        {
            //Debug.Log($"Host started successfully on port {transport.ConnectionData.Port}!");
        }
    }
    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
    }
    public void OpenLoadGameMenu()
    {
        //close main menu, open load menu
        mainMenu.SetActive(false);
        loadCharactersMenu.SetActive(true);
        loadCharacterMenuReturnButton.Select();
    }
    public void CloseLoadGameMenu()
    {
        mainMenu.SetActive(true); 
        loadCharactersMenu.SetActive(false);
        mainMenuLoadGameButton.Select();
    }
    public void DisplayNoFreeSlotsPopUp()
    {
        noCharacterSlotsFreePopUp.SetActive(true);
        noCharacterSlotsFreeOkayButton.Select();
    }
    public void CloseNoFreeSlotsPopUp()
    {
        noCharacterSlotsFreePopUp.SetActive(true);
        mainMenuNewGameButton.Select();
    }
    public void SelectCharacterSlot(CharacterSlot slot)
    {
        currentSelectedCharacterSlot = slot;
    }
    public void SelectNoSlot()
    {
        currentSelectedCharacterSlot = CharacterSlot.No_Slot;
        //loadCharacterMenuReturnButton.Select();
    }

    public void AttemptToDeleteCharacterSlot()
    {
        if(currentSelectedCharacterSlot != CharacterSlot.No_Slot)
        {
            deleteCharacterSlotPopUp.SetActive(true);
            deleteCharacterSlotConfirmButton.Select();
        }
    }
    public void CloseDeleteCharacterPopUp()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        loadCharacterMenuReturnButton.Select();
    }
    public void DeleteCharacterSlot()
    {
        deleteCharacterSlotPopUp.SetActive(false);

        WorldSaveGameManager.instance.DeleteCharacterSlot(currentSelectedCharacterSlot);

        //disbale then reable the character load screen to refresh the save slots on enable 
        loadCharactersMenu.SetActive(false);
        loadCharactersMenu.SetActive(true);

        loadCharacterMenuReturnButton.Select();
        
    }



}
