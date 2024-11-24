using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;
    public int worldSceneIndex = 1;
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
    }

    public IEnumerator LoadNewGame()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;

    }
}
