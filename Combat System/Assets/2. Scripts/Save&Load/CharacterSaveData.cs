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
    public float xPos, yPos, zPos;

    [Header("Attributes")]
    public int vitality;
    public int endurance;

    [Header("Resources")]
    public float currentHealth;
    public float currentStamina;


}

