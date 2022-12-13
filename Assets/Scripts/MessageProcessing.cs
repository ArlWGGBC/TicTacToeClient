using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageProcessing : MonoBehaviour
{
    
    public Messages Messages;
    public NetworkedClient _client;
    public HUD hud;

    public List<Replay> replays;
    string currentReplay = "";
    private void Start()
    {
       
        Messages = new Messages();
        _client = FindObjectOfType<NetworkedClient>();
        hud = FindObjectOfType<HUD>();
        replays = new List<Replay>();
    }

    public void ProcessMessage(string msg, int id)
    {
        
        string[] message = msg.Split(',', msg.Length);
        
        if (message[0] == Messages.Join)
        {
            //Assign values from message for clarity
            string room_Name = message[1];
            _client._currentRoom = room_Name;

            //Send join request to server.
            _client.SendMessageToHost((Messages.Join + "," + hud.GetRoomInput() + "," + _client.ConnectionID));



            //Tell UI to switch to game room - server already took care of data.
            hud.SwitchGameRoomScreen();
            hud.SetRoomName(_client.roomName);


        }
        else if (message[0] == Messages.Create)
        {
            //After sending create message to server, server send message back to client.

            //Assign values from message for clarity
            string room_Name = message[1];
            _client._currentRoom = room_Name;

            //Tell UI to switch to game room - server already took care of data.
            hud.SwitchGameRoomScreen();
            hud.SetRoomName(room_Name);

            //Instantiate fresh list to push strings inside - use this in PopulateRoomNames function. // UI LOOP
            List<string> playerIDs = new List<string>();

            playerIDs.Add(message[2]);

            //Pass player IDs to room names.
            hud.PopulateRoomNames(playerIDs);

            playerIDs.Clear();

        }
        else if (message[0] == Messages.PlayerCount)
        {
            hud.UpdatePlayersOnlineCount(Convert.ToInt32(message[1]));
        }
        else if (message[0] == Messages.Joined)
        {

            Debug.Log("Joined : " + message[1] + " : " + message[2]);


            //Instantiate fresh list to push strings inside - use this in PopulateRoomNames function. // UI LOOP
            List<string> playerIDs = new List<string>();

            //Grab player IDS passed from server(info from gameroom) and pass into list to give to UI function.
            playerIDs.Add(message[2]);
            playerIDs.Add(message[3]);

            //Pass player IDs to room names.
            hud.PopulateRoomNames(playerIDs);
            //hud.PopulateGameBoard(message[4],message[5], message[6],message[7],message[8],message[9],message[10],message[11],message[12])

            playerIDs.Clear();
        }
        else if (message[0] == Messages.Leave)
        {
            Debug.Log(message[2] + " Player : Left the game");
            _client._currentRoom = "";
            //Remove room name that was passed in.
            hud.ResetRoomHUD();
            hud.RemoveRoomName(message[2]);
        }
        else if (message[0] == Messages.MakeMove)
        {
            //makemove[0] , boardposition[1] , identity[2]

            foreach (var tile in hud.TTT_Tiles)
            {
                Debug.Log("Comparing : " + message[1] + " : " + tile.boardPosition);
                if (Convert.ToInt32(message[1]) == tile.boardPosition)
                {
                    Debug.Log("Setting Tile : " + message[1] + " to : " + message[2]);
                    tile.SetTile(message[2]);
                    break;
                }
            }
        }
        else if (message[0] == Messages.MessageC)
        {

            var player = ("Player " + message[2] + " : ");

            hud.AddChatMessage((player + message[1]));
        }
        
        else if (message[0] == Messages.GameStart)
        {
            Debug.Log("Identifier : " + message[1]);
            if (message[1] == identifier.O.ToString())
            {
                _client.identity = identifier.O;
            }
            else if (message[1] == identifier.X.ToString())
            {
                _client.identity = identifier.X;
            }
            else
            {
                _client.identity = identifier.N;
            }

            _client.SendMessageToHost(Messages.GameStart + "," + _client._currentRoom);

        }
        else if (message[0] == Messages.GameWon)
        {
            hud.ResetRoomHUD();
        }
        else if (message[0] == Messages.GetReplays)
        {
            //RECEIVED INFO
            //_message.GetReplays + ","  + roomName + "," + winner + "," + move.pos + "," + move.identifier, id


            Debug.Log("REPLAY MOVE : " + message[0] + ", " + message[1] + "," + message[3] + "," + message[4]);

            if (message[1] != currentReplay)
            {
                Debug.Log(message[1] + "Compared to :" + currentReplay);
                currentReplay = message[1];
                replays.Add(new Replay(message[2], message[1]));
                hud.AddReplay(message[1]);
            }

            if (replays.Count > 0)
            {
                foreach (var replay in replays)
                {
                    if (replay.roomname == currentReplay)
                    {
                        replay.moves.Add(new Move(message[3], message[4]));
                    }
                }
            }
        }
        else if (message[0] == Messages.CreateAccount)
        {
            if (message[1] == "1")
            {
                hud.SetUIText("Success");
                hud.SwitchLogInScreen();
            }
            else
            {
                hud.SetUIText("Account Already Exists");
            }
        }
        else if (message[0] == Messages.LoginAccount)
        {
            if (message[1] == "1")
            {
                hud.SetUITextLogin("Success");
                hud.SwitchStartScreen();
            }
            else
            {
                hud.SetUITextLogin("Invalid Credentials");
            }
        }
    }
}

#region Structs

public struct Move
{

    public Move(string position, string id)
    {
        pos = position;
        identifier = id;
    }


    public string pos;
    public string identifier;
}

public struct Replay
{
    public List<Move> moves;

    public Replay(string winner, string roomname)
    {
        moves = new List<Move>();
        this.roomname = roomname;
        this.winner = winner;
    }


    public string roomname;
    public string winner;
}


#endregion
