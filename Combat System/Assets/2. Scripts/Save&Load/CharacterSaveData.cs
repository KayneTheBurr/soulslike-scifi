using UnityEngine;

[System.Serializable]

public class CharacterSaveData
{
    [Header("Character Name")]
    public string characterName;

    [Header("Time Played")]
    public float timePlayedSec;

    [Header("Position")]
    public float xPos, yPos, zPos;


}

