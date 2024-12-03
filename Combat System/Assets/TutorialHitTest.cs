using UnityEngine;

public class TutorialHitTest : MonoBehaviour
{
    AICharacterManager aiCharacter;

    private void Awake()
    {
        aiCharacter = GetComponent<AICharacterManager>();
    }

    private void Update()
    {
        if(aiCharacter.aiNetworkManager.currentHealth.Value <= 0)
        {
            TutorialUIManager.instance.EnemyKilled();


            Destroy(gameObject);
        }
    }
}
