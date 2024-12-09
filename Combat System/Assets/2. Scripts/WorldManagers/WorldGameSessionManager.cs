using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGameSessionManager : MonoBehaviour
{
    public static WorldGameSessionManager instance;

    [Header("Active Players In Session")]
    public List<PlayerManager> players = new List<PlayerManager>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void AddPlayerToActivePlayersList(PlayerManager player)
    {
        //check the list, if it does not already contain the player then add them
        if(!players.Contains(player))
        {
            players.Add(player);
        }
        //check the list for null slots
        for (int i = players.Count - 1; i > -1; i--)
        {
            players.RemoveAt(i);
        }
    }
    public void RemovePlayerToActivePlayersList(PlayerManager player)
    {
        if(players.Contains(player))
        {
            players.Remove(player);
        }
        
        //check the list for null slots
        for (int i = players.Count - 1; i > -1; i--)
        {
            players.RemoveAt(i);
        }
    }
}
