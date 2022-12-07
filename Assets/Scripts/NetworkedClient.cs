using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerData))]
public class NetworkedClient : MonoBehaviour
{
    #region Network Variables

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 8000;
    byte error;
    bool isConnected = false;
    int ourClientID;

    public identifier identity;
    public string _currentRoom;

    public Messages Messages;

    #endregion

    #region Replay

    public Transform Replay_BoardParent;
    public BoardTile[] Replay_TTT_Tiles;

    public List<Replay> replays;

    string currentReplay = "";

    #endregion


    public StateMachine _currentState;

    //Can transition to HUD -> Possibly wrap this in a separate class -> init the TTT_Tiles with constructor and hold a reference to it in HUD class.
    public Transform BoardParent;
    public BoardTile[] TTT_Tiles;




    [SerializeField] public HUD hud;

    //Current account credentials local
    protected string accountName;

    //protected string accountPassword;

    private PlayerData _playerData;

    //Is connection id
    protected int accountID;

    //roomName local
    protected string roomName;


    //Used for input on login screen
    //protected string LoginInputName;
    //protected string LoginInputPassword;

    //booleans for account creation screen
    //protected bool AccountNameSet = false;
    //protected bool AccountPasswordSet = false;

    //connected to account creation booleans
    //protected bool CanCreate = true;





    public void SetState(StateMachine state)
    {

        _currentState = state;
        _currentState.Start();
    }
//----------------------------------------------------------


    #region Unity Functions

// Start is called before the first frame update
    void Start()
    {


        Debug.Log(Application.persistentDataPath);

        TTT_Tiles = BoardParent.GetComponentsInChildren<BoardTile>();
        Replay_TTT_Tiles = Replay_BoardParent.GetComponentsInChildren<BoardTile>();
        
        
        _playerData = this.GetComponentInChildren<PlayerData>();
        
        if (_playerData == null)
        {
            this.AddComponent<PlayerData>();
            _playerData = this.GetComponentInChildren<PlayerData>();
        }

        for (int i = 0; i < TTT_Tiles.Length; i++)
        {
            TTT_Tiles[i].boardPosition = i + 1;
            Replay_TTT_Tiles[i].boardPosition = i + 1;
        }


        Messages = new Messages();

        //Get replays.


        Connect();
        SetState(new CreateAccountState(this));

        replays = new List<Replay>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SendMessageToHost("Hello from client" + accountName);

        UpdateNetworkConnection();
    }


    #endregion
    

    #region Networking Functions

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID,
                out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    //=SendDataToHost();

                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
                case NetworkEventType.BroadcastEvent:
                    break;

                default:
                    break;
            }
        }
    }

    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID =
                NetworkTransport.Connect(hostID, "10.0.238.71", socketPort, 0,
                    out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;
                Debug.Log("Connected, id = " + connectionID);
            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
        LeaveGame();
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    public void SendDataToHost(char[] msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        // message[0] == Signifier, message[1] == Room Name, message[2] == Room owner ID(player1)

        string[] message = msg.Split(',', msg.Length);


        if (message[0] == Messages.Join)
        {
            //Assign values from message for clarity
            string room_Name = message[1];
            _currentRoom = room_Name;

            //Send join request to server.
            SendMessageToHost((Messages.Join + "," + hud.GetRoomInput() + "," + connectionID));



            //Tell UI to switch to game room - server already took care of data.
            hud.SwitchGameRoomScreen();
            hud.SetRoomName(roomName);


        }
        else if (message[0] == Messages.Create)
        {
            //After sending create message to server, server send message back to client.

            //Assign values from message for clarity
            string room_Name = message[1];
            _currentRoom = room_Name;

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
            _currentRoom = "";
            //Remove room name that was passed in.
            hud.ResetRoomHUD();
            hud.RemoveRoomName(message[2]);
        }
        else if (message[0] == Messages.MakeMove)
        {
            //makemove[0] , boardposition[1] , identity[2]

            foreach (var tile in TTT_Tiles)
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
            if (message[1] == identifier.O.ToString())
            {
                identity = identifier.O;
            }
            else if (message[1] == identifier.X.ToString())
            {
                identity = identifier.X;
            }
            else
            {
                identity = identifier.N;
            }

            SendMessageToHost(Messages.GameStart + "," + _currentRoom);

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

    

    #endregion


    #region Getters and Setters
    
    public bool IsConnected()
    {
        return isConnected;
    }
    public string RoomName
    {
        get { return roomName; }
        set { roomName = value; }
    }
    

    public int AccountID
    {
        get { return accountID; }
        set { accountID = value; }
    }

    public int ConnectionID
    {
        get { return connectionID; }
        set { connectionID = value; }
    }
    

    //End getters and setters----------------------------------------



    #endregion


    #region User Action Functions

    public void LeaveGame()
    {
        hud.OnLoggedInScreen();
        SendMessageToHost(Messages.Leave + "," + _currentRoom + "," + connectionID);

        hud.ResetRoomHUD();
    }

    public void CreateRoom()
    {
        _currentRoom = hud.GetRoomInput();
        SendMessageToHost((Messages.Create + "," + hud.GetRoomInput() + "," + connectionID));


    }

    public void GetReplay()
    {
        //Send message to server - We want all the replays.
        SendMessageToHost(Messages.GetReplays);

    }



    public void AddReplay()
    {



        //Hud.PopulateList();

    }


    #endregion
    
    
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

}
