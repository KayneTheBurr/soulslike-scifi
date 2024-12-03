using UnityEngine;

public class LearnJumpTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TutorialUIManager.instance.learnJump = true;
        Destroy(gameObject);
    }
}
