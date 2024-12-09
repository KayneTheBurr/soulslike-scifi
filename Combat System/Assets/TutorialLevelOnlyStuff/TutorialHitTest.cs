using UnityEngine;

public class TutorialHitTest : MonoBehaviour
{
    AICharacterManager aiCharacter;

    public bool sentDeathData = false;

    private void Awake()
    {
        aiCharacter = GetComponent<AICharacterManager>();
    }

    private void Start()
    {
        TutorialUIManager.instance.CountEnemies();
    }

    private void Update()
    {
        if(aiCharacter.aiNetworkManager.currentHealth.Value <= 0)
        {
            if (!sentDeathData)
            {
                sentDeathData = true;
                TutorialUIManager.instance.EnemyKilled();
            }
                
 
        }
    }
}
