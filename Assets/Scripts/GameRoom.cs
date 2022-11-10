using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom
{
    public string roomName;
    public string gameType;
    public int player1ID;
    public int player2ID;
    public List<int> spectatorIDs;
    
    

    public GameRoom(string roomN, string gameT)
    {
        roomName = roomN;
        gameType = gameT;
    }
}
