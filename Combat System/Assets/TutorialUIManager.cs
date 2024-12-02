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
    public bool learnMove= true;
    public bool learnJump = false;
    public bool learnRoll = false;
    public bool learnSprint = false;
    public bool learnAttack = false;
    public bool flee = false;

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
        if(learnMove)
        {
            learnMove = false;
            //tell them how to move/aim
            infoText.text = "Use Mouse to look, WASD to Move";
        }
        if (learnJump)
        {
            currentTutorialStep++;
            learnJump = false;
            infoText.text = "Jump over laser with F";
        }
        if(learnRoll)
        {
            currentTutorialStep++;
            learnRoll = false;
            infoText.text = "Roll under laser with [SPACE]";
        }
        if(learnSprint)
        {
            learnSprint = false;
            currentTutorialStep++;
            infoText.text = "Sprint to jump farther by holding down SPACE";
        }
        if(learnAttack)
        {
            learnAttack = false;
            currentTutorialStep++;
            infoText.text = "Right Click to perform light attack";
        }
        if(flee)
        {
            flee = false;
            currentTutorialStep++;
            infoText.text = "Avoid all enemies and get to the end!";
        }
        if(player.playerNetworkManager.currentHealth.Value <= 0)
        {
            StartCoroutine(Respawn());
        }
    }
    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(4);
        player.gameObject.transform.position = Vector3.zero;
        player.respawnCharacter = true;
        
    }

    public void EnemyKilled()
    {
        currentTutorialStep++;
        infoText.text = "Roll while in-air parkour(ish)";
    }
    public void WinTutorial()
    {
        currentTutorialStep++;
        infoText.text = "You Win";
    }

}
