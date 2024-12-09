using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    


    [Header("Characters")]
    [SerializeField] private List<AICharacterSpawner> aiCharacterSpawners;
    //[SerializeField] GameObject[] aICharacters;
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
        
    }
    private void Update()
    {

    }

    public void SpawnCharacter(AICharacterSpawner aiSpawner)
    {
        if(NetworkManager.Singleton.IsServer)
        {
            aiCharacterSpawners.Add(aiSpawner);
            aiSpawner.AttemptToSpawnCharacter();
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
