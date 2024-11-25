using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;
    [SerializeField] private PlayerManager player;

    [Header("Save/Load")]
    [SerializeField] bool saveGame, loadGame;


    [Header("World Scene Index")]
    [SerializeField] public int worldSceneIndex = 1;

    [Header("Save Data Writer")]
    private WriteSaveData saveDataWriter;

    [Header("Current Character Data")]
    public CharacterSlot currentCharacterSlotUsed;
    public CharacterSaveData currentCharacterData;
    private string fileName;

    [Header("CharacterSlots")]
    public CharacterSaveData characterSlot01, characterSlot02, characterSlot03, characterSlot04, characterSlot05,
                                characterSlot06, characterSlot07, characterSlot08, characterSlot09, characterSlot10;
    private void Awake()
    {
        //one at a time 
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllCharacterProfiles();
    }
    private void Update()
    {
        if(saveGame)
        {
            saveGame = false;
            SaveGame();
        }
        if(loadGame)
        {
            loadGame = false; 
            LoadGame();
        }
    }
    public string DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot slot)
    {
        string fileName = "";
        switch(slot)
        {
            case CharacterSlot.CharacterSlot_01:
                fileName = "characterSlot_01";
                break;
            case CharacterSlot.CharacterSlot_02:
                fileName = "characterSlot_02";
                break;
            case CharacterSlot.CharacterSlot_03:
                fileName = "characterSlot_03";
                break;
            case CharacterSlot.CharacterSlot_04:
                fileName = "characterSlot_04";
                break;
            case CharacterSlot.CharacterSlot_05:
                fileName = "characterSlot_05";
                break;
            case CharacterSlot.CharacterSlot_06:
                fileName = "characterSlot_06";
                break;
            case CharacterSlot.CharacterSlot_07:
                fileName = "characterSlot_07";
                break;
            case CharacterSlot.CharacterSlot_08:
                fileName = "characterSlot_08";
                break;
            case CharacterSlot.CharacterSlot_09:
                fileName = "characterSlot_09";
                break;
            case CharacterSlot.CharacterSlot_10:
                fileName = "characterSlot_10";
                break;
        }
        return fileName;

    }
    public void CreateNewGame()
    {
        //create a new file with a file name depending on slot we are using
        fileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(currentCharacterSlotUsed);

        currentCharacterData = new CharacterSaveData();
    }
    public void LoadGame()
    {
        //load a previous game with a file depending on which slot we are using
        fileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(currentCharacterSlotUsed);

        saveDataWriter = new WriteSaveData();
        //generally works on multiple machine types 
        saveDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveDataWriter.saveFileName = fileName;
        currentCharacterData = saveDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());
    }
    public void SaveGame()
    {
        fileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(currentCharacterSlotUsed);

        saveDataWriter = new WriteSaveData();
        //generally works on multiple machine types 
        saveDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveDataWriter.saveFileName = fileName;

        //pass player info from game to the save file
        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        //write that info onto a json file saved on the (this) machine
        saveDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }
    //load all character profiles on device when starting game 
    private void LoadAllCharacterProfiles()
    {
        saveDataWriter = new WriteSaveData();
        saveDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_01);
        characterSlot01 = saveDataWriter.LoadSaveFile();

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_02);
        characterSlot02 = saveDataWriter.LoadSaveFile();

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_03);
        characterSlot03 = saveDataWriter.LoadSaveFile();

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_04);
        characterSlot04 = saveDataWriter.LoadSaveFile();

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_05);
        characterSlot05 = saveDataWriter.LoadSaveFile();

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_06);
        characterSlot06 = saveDataWriter.LoadSaveFile();

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_07);
        characterSlot07 = saveDataWriter.LoadSaveFile();

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_08);
        characterSlot08 = saveDataWriter.LoadSaveFile();

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_09);
        characterSlot09 = saveDataWriter.LoadSaveFile();

        saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_10);
        characterSlot10 = saveDataWriter.LoadSaveFile();

    }
    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;

    }
}
