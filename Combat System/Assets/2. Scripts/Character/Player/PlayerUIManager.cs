using Unity.Netcode;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [HideInInspector] public PlayerUIHUDManager playerHUDManager;

    //network join stuff
    [SerializeField] bool startGameAsClient;

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

        playerHUDManager = GetComponentInChildren<PlayerUIHUDManager>();

    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(startGameAsClient)
        {
            startGameAsClient = false;
            NetworkManager.Singleton.Shutdown(); //start as host, shutdown to restart as client
            NetworkManager.Singleton.StartClient();
        }
    }

}
