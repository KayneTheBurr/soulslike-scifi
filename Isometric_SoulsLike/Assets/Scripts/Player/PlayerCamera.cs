using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Camera cam;
    public static PlayerCamera instance;

    private void Awake()
    {
        if(instance == null)
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
