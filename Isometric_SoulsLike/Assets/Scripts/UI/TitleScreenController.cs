using UnityEngine;
using Unity.Netcode;

public class TitleScreenController : MonoBehaviour
{
    public void StartAsNetworkHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void StartNewGame()
    {
        WorldSaveGameManager.instance.StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
    }
}
