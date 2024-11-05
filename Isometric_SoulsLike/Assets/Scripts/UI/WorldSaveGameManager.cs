using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;
    [SerializeField] int worldSceneIndex;

    private void Awake()
    {
        //one save amager at a time 

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
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(1);
        yield return null;
    }
    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}
