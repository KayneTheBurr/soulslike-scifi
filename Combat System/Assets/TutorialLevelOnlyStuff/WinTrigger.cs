using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TutorialUIManager.instance.WinTutorial();
    }
}
