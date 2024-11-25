using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{

    [Header("Menus")]
    public GameObject mainMenu, loadCharactersMenu;
    [Header("Buttons")]
    public Button loadCharacterMenuReturnButton, mainMenuLoadGameButton;

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    
    public void StartNewGame()
    {
        WorldSaveGameManager.instance.CreateNewGame();
        StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
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
}
