using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NetworkedClient : MonoBehaviour
{
    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 8000;
    byte error;
    bool isConnected = false;
    int ourClientID;

    private MessageType _message;
    public StateMachine _currentState;

    public TicTacToeBoard TicTacToeBoard;
    
    
    public string _currentRoom;
   
    
    [SerializeField] public HUD hud;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    
    
    
    
    //Current account credentials local
    protected string accountName;
    protected string accountPassword;
    //Is connection id
    protected int accountID;
    
    //Used for input on login screen
    protected string LoginInputName;
    protected string LoginInputPassword;

    //booleans for account creation screen
    protected bool AccountNameSet = false;
    protected bool AccountPasswordSet = false;
    
    //connected to account creation booleans
    protected bool CanCreate = true;

    
    //roomName local
    protected string roomName;

    public bool isO;


    public void SetState(StateMachine state)
    {
        
        _currentState = state;
        _currentState.Start();
    }

    
    // Start is called before the first frame update
    void Start()
    {

        TicTacToeBoard = FindObjectOfType<TicTacToeBoard>();
        
        
        _message = new MessageType();
        
        
        Connect();
        SetState(new CreateAccountState(this));
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
            SendMessageToHost("Hello from client" + accountName);

        UpdateNetworkConnection();
    }

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
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

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
                
                default :
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

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.16", socketPort, 0, out error); // server is local on network

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
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char),  out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        // message[0] == Signifier, message[1] == Room Name, message[2] == Room owner ID(player1)
        
        string[] message = msg.Split(',', msg.Length);

        _textMeshProUGUI.text = ("Recieved: " + msg);

        if (message[0] == _message.Join)
        {
            //Assign values from message for clarity
            string room_Name = message[1];
            _currentRoom = room_Name;
            
            //Send join request to server.
            SendMessageToHost((_message.Join + "," + hud.GetRoomInput() + "," + connectionID));
            
                
            
            //Tell UI to switch to game room - server already took care of data.
            hud.SwitchGameRoomScreen();
            hud.SetRoomName(roomName);
            
           
        }
        else if (message[0] == _message.Create)
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

            isO = false;
            //Pass player IDs to room names.
            hud.PopulateRoomNames(playerIDs);
            
            playerIDs.Clear();

        }
        else if (message[0] == _message.PlayerCount)
        {
            hud.UpdatePlayersOnlineCount(Convert.ToInt32(message[1]));
        }
        else if (message[0] == _message.Joined)
        {
            
            Debug.Log("Joined : " + message[1] + " : " + message[2]);
            
            
            //Instantiate fresh list to push strings inside - use this in PopulateRoomNames function. // UI LOOP
            List<string> playerIDs = new List<string>();
            
            //Grab player IDS passed from server(info from gameroom) and pass into list to give to UI function.
                    playerIDs.Add(message[2]);
                    playerIDs.Add(message[3]);
                    
                    //Pass player IDs to room names.
                    hud.PopulateRoomNames(playerIDs);
                    
                    playerIDs.Clear();
        }
        else if (message[0] == _message.Leave)
        {
            Debug.Log(message[2] + " Player : Left the game");
            //Remove room name that was passed in.
            hud.RemoveRoomName(message[2]);
        }
        else if (message[0] == _message.MakeMove)
        {
            
        }
      


    }


    public void LeaveGame()
    {
        hud.OnLoggedInScreen();
        SendMessageToHost(_message.Leave + "," + _currentRoom + "," + connectionID);
        
        hud.ResetRoomHUD();
    }
    public void CreateRoom()
    {
        _currentRoom = hud.GetRoomInput();
        SendMessageToHost((_message.Create + "," + hud.GetRoomInput() + "," + connectionID));
        

    }
    
    
    public bool IsConnected()
    {
        return isConnected;
    }

    
    //State machine virtual functions-----------------------------
    public void OnSubmitName()
    {
        StartCoroutine(_currentState.SetName());
    }
    
    public void OnSubmitPassword()
    {
        StartCoroutine(_currentState.SetPassword());
    }
    
    public void Check()
    {
        StartCoroutine(_currentState.CheckAccount());
    }
    
       
   
    //-----------------------------------------------------------------
    
    //Create the account file and set appropriate variables.
    public void CreateAccount()
    {
        Debug.Log("Creating account");
        if (!CanCreate)
            return;
        
        Debug.Log("Successfull");
        
        
        string filePath = Application.persistentDataPath + "/" + accountName + ".sav";

        if (File.Exists(filePath))
        {
            Debug.Log("User already exists");
            return;
        }
            
        
        StreamWriter sw = new StreamWriter(filePath);
        sw.WriteLine("name" + "," + accountName);
        sw.WriteLine("password" + "," + accountPassword);
        sw.WriteLine("clientID" + "," + ourClientID);
        sw.WriteLine("UniqueID" + Random.Range(1000, 3000));
        
        sw.Close();
        
        hud.SetUIText("Account Created");
        CreateLoginScreen();
    }

    //Check user input against file to determine whether login is successful, then set state if success.
    public void LoginAccount()
    {
        bool nameCorrect = false;
        bool passwordCorrect = false;
        
        string saveFilePath = Application.persistentDataPath + "/account.sav";
        var fileInfo = Directory.GetFiles(Application.persistentDataPath);


        if (fileInfo.Length <= 0)
            return;
        
        foreach (var file in fileInfo)
        {
            StreamReader sr = new StreamReader(file);

            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] stats = line.Split(',');
            
                if (stats[0] == "name")
                {
                    if (stats[1] == AccountNameInput)
                    {
                        nameCorrect = true;
                    }
                }
                else if (stats[0] == "password")
                {
                    if (stats[1] == AccountPasswordInput)
                    {
                        passwordCorrect = true;
                    }
                }
            }

            if (nameCorrect && passwordCorrect)
            {
                SetState(new LoggedInState(this));
                break;
            }
            
            sr.Close();
        }
        
        
        hud.SetUITextLogin("Incorrect Credentials");

        
    }
    


    //Creating and Logging into account functionality + helper functions
    public void LoginName()
    {
        AccountNameInput = hud.GetLoginNameInput();
    }
    
    public void LoginPassword()
    {
        AccountPasswordInput = hud.GetLoginPasswordInput();
    }
    
    public void CreateAccountScreen()
    {
        hud.SwitchCreateAccountScreen();
    }
    
    public void CreateLoginScreen()
    {
        hud.SwitchLogInScreen();
    }

   
    
    //Getters and setters--------------------------------------------
    
    void ResetVariables()
    {
        NameSet = false;
        PasswordSet = false;
    }
    public string RoomName
    {
        get { return roomName; }
        set { roomName = value;  }
    }
    public string AccountName
    {
        get { return accountName; }
        set { accountName = value; }
    }
    
    public string AccountNameInput
    {
        get { return LoginInputName; }
        set { LoginInputName = value; }
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
    
    public string AccountPasswordInput
    {
        get { return LoginInputPassword; }
        set { LoginInputPassword = value; }
    }
    
    public bool canCreateAccount
    {
        get { return CanCreate; }
        set { CanCreate = value; }
    }
    
    public string AccountPassword
    {
        get { return accountPassword; }
        set { accountPassword = value; }
    }
    
    public bool NameSet
    {
        get { return AccountNameSet; }
        set { AccountNameSet = value; }
    }

    public bool PasswordSet
    {
        get { return AccountPasswordSet; }
        set { AccountPasswordSet = value; }
    }
    
    //End getters and setters----------------------------------------
}
