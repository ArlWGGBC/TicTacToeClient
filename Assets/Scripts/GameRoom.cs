using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameRoom
{
    public string roomName;
    public string gameType;
    public string player1ID;
    public string player2ID;
    public List<string> spectatorIDs;
    public List<string> playerIDs;

    public List<BoardTile> TicTacToeBoardTiles;
    
    

    public GameRoom(string roomN, string gameT)
    {
        player1ID = null;
        player2ID = null;
        roomName = roomN;
        gameType = gameT;
        

        playerIDs = new List<string>();
        spectatorIDs = new List<string>();
        TicTacToeBoardTiles = new List<BoardTile>();
    }
}
