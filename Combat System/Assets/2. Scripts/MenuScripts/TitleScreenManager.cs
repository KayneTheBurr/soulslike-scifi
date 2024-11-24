using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    
    void Start()
    {
        
    }
    public void StartNewGame()
    {
        StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
    }
    
}
