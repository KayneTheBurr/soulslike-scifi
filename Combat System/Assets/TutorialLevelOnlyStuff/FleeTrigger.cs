using UnityEngine;

public class FleeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TutorialUIManager.instance.flee = true;
        Destroy(gameObject);
    }
}
