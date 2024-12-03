using UnityEngine;

public class LearnRollTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TutorialUIManager.instance.learnRoll = true;
        Destroy(gameObject);
    }
}
