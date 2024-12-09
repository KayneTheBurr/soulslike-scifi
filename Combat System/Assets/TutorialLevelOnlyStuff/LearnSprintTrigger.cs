using UnityEngine;

public class LearnSprintTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TutorialUIManager.instance.learnSprint = true;
        Destroy(gameObject);
    }
}
