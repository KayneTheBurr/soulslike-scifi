using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [HideInInspector] public PlayerUIHUDManager playerHUDManager;
    [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;

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
        playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();

    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator RestartAsClient()
    {
        Debug.Log("Switching from temporary Host to Client...");

        // If running as host, shut it down
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Shutting down temporary Host...");
            NetworkManager.Singleton.Shutdown();
        }

        // Wait for the host shutdown to complete
        while (NetworkManager.Singleton.IsListening)
        {
            yield return null;
        }

        // Reconfigure the client to connect to the main host
        Debug.Log("Configuring client to connect to main Host...");
        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        //transport.ConnectionData.Address = "127.0.0.1"; // Localhost for testing
        transport.ConnectionData.Port = 7778;          // Main host's port

        Debug.Log($"Starting as Client, connecting to {transport.ConnectionData.Address}:{transport.ConnectionData.Port}");
        if (!NetworkManager.Singleton.StartClient())
        {
            Debug.LogError("Failed to start Client! Check connection details.");
            yield break;
        }

        Debug.Log("Client started successfully!");
    }


    private void Update()
    {
        if(startGameAsClient)
        {
            Debug.Log("Switching to Client mode...");
            startGameAsClient = false;

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Shutting down Host...");
                NetworkManager.Singleton.Shutdown();
            }

            // Restart as Client
            StartCoroutine(RestartAsClient());


            //First try attempts at connection code
            //startGameAsClient = false;
            //NetworkManager.Singleton.Shutdown(); //start as host, shutdown to restart as client
            //Debug.Log("Shutting Down as Host");
            //NetworkManager.Singleton.StartClient();
            //Debug.Log("Starting as Client");
        }
    }

}
