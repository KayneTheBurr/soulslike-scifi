using UnityEngine;
using System;
using System.IO;
using System.Linq.Expressions;

public class WriteSaveData
{
    public string saveDataDirectoryPath = "";
    public string saveFileName = "";

    //before we make a new save, check to see if this character slot already exists
    public bool CheckToSeeIfFileExists()
    {
        if(File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    // used to delete character save files from a slot 
    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    //used to create a save file upon starting a new game
    public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
    {
        //make a path to save the file
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        try
        {
            //create the directory the file will be written to if it doesnt already exist
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("Creating save file at save path: " + savePath);

            //serialize the C# game data object into json format 
            string dataToStore = JsonUtility.ToJson(characterData, true);

            //write the file onto our system
            using(FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using(StreamWriter fileWriter = new StreamWriter(stream))
                {
                    fileWriter.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error trying to save the game, " + ex);
        }
    }

    //load a game from a previous save
    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData characterData = null;

        //make a path to load the file (on computer)
        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if(File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                // De-serialize the data from json to C#
                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("No save file found, " + ex);
            }
        }
        return characterData;
    }
}
