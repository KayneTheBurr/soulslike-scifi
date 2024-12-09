using UnityEngine;
using Unity.Netcode;


public class AICharacterSpawner : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] GameObject characterGameObject;
    [SerializeField] GameObject instantiatedObject;

    private void Awake()
    {
        
    }

    private void Start()
    {
        WorldAIManager.instance.SpawnCharacter(this);
        gameObject.SetActive(false);
    }

    public void AttemptToSpawnCharacter()
    {
        if(characterGameObject != null)
        {
            instantiatedObject = Instantiate(characterGameObject);
            instantiatedObject.transform.position = transform.position;
            instantiatedObject.transform.rotation = transform.rotation;

            instantiatedObject.GetComponent<NetworkObject>().Spawn();
        }
    }

}
