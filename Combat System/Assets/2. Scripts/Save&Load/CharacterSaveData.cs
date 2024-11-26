using UnityEngine;

[System.Serializable]

public class CharacterSaveData
{
    [Header("Scene Index")]
    public int sceneIndexNumber = 1;


    [Header("Character Name")]
    public string characterName = "Character";

    [Header("Time Played")]
    public float timePlayedSec = 0;

    [Header("Position")]
    public float xPos, yPos, zPos;


}

