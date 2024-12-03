using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Debugging")]
    [SerializeField] bool despawnAllCharacters = false;
    [SerializeField] bool spawnAllCharacters = false;


    [Header("Characters")]
    [SerializeField] GameObject[] aICharacters;
    [SerializeField] List<GameObject> spawnedInCharacters;

    private void Awake()
    {
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
        if(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            StartCoroutine(WaitForSceneToLoad());
        }
    }
    private void Update()
    {
        if(despawnAllCharacters)
        {
            despawnAllCharacters = false;
            DespawnAllCharacters();
        }
        if(spawnAllCharacters)
        {
            spawnAllCharacters = false;
            SpawnAllCharacters();
        }
    }

    private IEnumerator WaitForSceneToLoad()
    {
        while(!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
        SpawnAllCharacters();
    }
    private void SpawnAllCharacters()
    {
        Debug.Log("Spawn All Enemies!");

        foreach (var character in aICharacters)
        {
            GameObject instantiatedCharacter = Instantiate(character);
            instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
            spawnedInCharacters.Add(instantiatedCharacter);
        }
    }
    private void DespawnAllCharacters()
    {
        foreach (var character in spawnedInCharacters)
        {
            character.GetComponent<NetworkObject>().Despawn();
        }
    }
    private void DisableAllCharacters()
    {
        //disable character game onjects, sync disabled status on network
        //disable gameobjects for clients upon connecting, if disabled status is true
        //can be used to disable characters that are far away from players to save memory
        //characters can be split into areas (area00, area01, etc)
    }



}
