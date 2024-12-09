using RPGCharacterAnims.Actions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TutorialUIManager : MonoBehaviour
{
    public static TutorialUIManager instance;

    public PlayerManager player;

    public TextMeshProUGUI infoText;
    public int currentTutorialStep = 1;
    public int totalEnemies = 0;
    public int enemiesKilled = 0;
    public bool winGame = false;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        player = FindFirstObjectByType<PlayerManager>();
    }
    private void Update()
    {
        infoText.text = "Killed " + enemiesKilled + " / " + totalEnemies;
        if(player.playerNetworkManager.currentHealth.Value <= 0)
        {
            StartCoroutine(Respawn());
        }
        
        if (enemiesKilled >= totalEnemies)
        {
            if(!winGame)
            {
                winGame = true;
                WinTutorial();
            }
        }
    }
    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2);
        if(player != null)
        {
            player.gameObject.transform.position = Vector3.zero;
            player.respawnCharacter = true;
        }
    }
    public void CountEnemies()
    {
        totalEnemies++;
    }
    public void EnemyKilled()
    {
        enemiesKilled++;
    }
    public void WinTutorial()
    {
        
        infoText.text = "You Win";
    }

}
