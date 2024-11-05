using UnityEngine;
using Unity.Netcode;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [Header("Network Join")]
    [SerializeField] bool startGameASClient;
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
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if(startGameASClient)
        {
            startGameASClient = false;
            //shuts down first bc started as a host on title screen 
            NetworkManager.Singleton.Shutdown();
            //restart as a client after
            NetworkManager.Singleton.StartClient();

        }
    }

}
