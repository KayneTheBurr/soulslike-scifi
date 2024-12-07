using UnityEngine;

public class WorldSFXManager : MonoBehaviour
{
    public static WorldSFXManager instance;

    [Header("Action SFX")]
    public AudioClip rollSFX;


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
}
