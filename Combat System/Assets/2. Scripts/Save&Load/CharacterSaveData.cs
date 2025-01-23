using UnityEngine;

[System.Serializable]

public class CharacterSaveData
{
    [Header("Scene Index")]
    public int sceneIndexNumber;


    [Header("Character Name")]
    public string characterName = "Character";

    [Header("Time Played")]
    public float timePlayedSec = 0;

    [Header("Position")]
    public float xPos;
    public float yPos;
    public float zPos;

    [Header("Attributes")]
    public int vitality;
    public int endurance;

    [Header("Resources")]
    public float currentHealth;
    public float currentStamina;

    [Header("Bosses")]
    public SerializedDictionary<int, bool> bossesAwakened; //int is boss ID, bool is boss room entered/boss awakened 
    public SerializedDictionary<int, bool> bossesDefeated; //int is boss ID, bool is boss defeated or not 

    public CharacterSaveData()
    {
        bossesAwakened = new SerializedDictionary<int, bool>();
        bossesDefeated = new SerializedDictionary<int, bool>();
    }

}

