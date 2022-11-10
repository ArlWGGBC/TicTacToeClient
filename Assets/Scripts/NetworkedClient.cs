using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using Unity.VisualScripting;
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
    
    
    public StateMachine _currentState;
    
    [SerializeField]
    public List<GameRoom> _gameRooms; 
    public GameRoom _currentRoom;
   
    
    [SerializeField] public HUD hud;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    
    
    
    
    //Current account credentials local
    protected string accountName;
    protected string accountPassword;
    //Is connection id
    protected int accountID;
    
    //Used for input on login screen
    protected string loginInputName;
    protected string loginInputPassword;

    //booleans for account creation screen
    protected bool accountNameSet = false;
    protected bool accountPasswordSet = false;
    
    //connected to account creation booleans
    protected bool canCreate = true;

    
    //roomName local
    protected string roomName;

    
    

    public void SetState(StateMachine state)
    {
        
        _currentState = state;
        _currentState.Start();
    }

    
    // Start is called before the first frame update
    void Start()
    {

        
        _gameRooms = new List<GameRoom>();
        
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
                    //SendDataToHost();
                        
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

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.11", socketPort, 0, out error); // server is local on network

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
        string[] message = msg.Split(',', msg.Length);

        Debug.Log(msg);

        _textMeshProUGUI.text = ("Recieved: " + msg);

        switch (message[0])
        {
            case "GAMEROOM" :
                
                if (message[2] == "Created")
                {
                    var gameRoom = new GameRoom(message[1], "Default");
                    gameRoom.player1ID = id;
                    //Create new game room
                    //Set current Room to room we created.
                    _currentRoom = gameRoom;
                    //Tell UI to change and populate screen.
                    SetupRoom();
                }
                else if (message[2] == "Join")
                {
                    var gameRoom = new GameRoom(message[1], "Default");
                    _currentRoom = gameRoom;
                    hud.SwitchGameRoomScreen();
                    SetupRoom();
                    JoinRoom(message[1], message[3], message[4]);
                }
                break;
            case "PLAYERCOUNT" :
                hud.UpdatePlayersOnlineCount(Convert.ToInt32(message[1]));
                break;
            default:
                Debug.Log(msg);
                break;
            
        }
        
        
    }

    
    public void SetupRoom()
    {
        
        hud.SetRoomName(hud.GetRoomInput());
        hud.SwitchGameRoomScreen();

        hud.PopulateRoomNames(loginInputName);
        
        
        
        //Get names to populate in new room... Need to parse through information and set it.
        
    }
    
    public void CreateRoom()
    {
        //SetRoomName();
        //STEP 1 : When clicking on create game room - send room name to server.
        SendMessageToHost("GAMEROOM," + hud.GetRoomInput());
    }

    public void JoinRoom(string roomName, string player1, string player2)
    {
        _currentRoom.roomName = roomName;
        
        if(String.IsNullOrEmpty(player1))
            _currentRoom.player1ID = Convert.ToInt32(player1);
        if(String.IsNullOrEmpty(player2))
            _currentRoom.player2ID = Convert.ToInt32(player2);
        _currentRoom.gameType = "Default";
   
        hud.SetRoomName(hud.GetRoomInput());
        hud.PopulateRoomNames(player1);
        hud.PopulateRoomNames(player2);
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
        if (!canCreate)
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
        }
        
        
        hud.SetUITextLogin("Incorrect Credentials");


    }
    

void ResetVariables()
    {
        NameSet = false;
        PasswordSet = false;
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
        get { return loginInputName; }
        set { loginInputName = value; }
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
        get { return loginInputPassword; }
        set { loginInputPassword = value; }
    }
    
    public bool canCreateAccount
    {
        get { return canCreate; }
        set { canCreate = value; }
    }
    
    public string AccountPassword
    {
        get { return accountPassword; }
        set { accountPassword = value; }
    }
    
    public bool NameSet
    {
        get { return accountNameSet; }
        set { accountNameSet = value; }
    }

    public bool PasswordSet
    {
        get { return accountPasswordSet; }
        set { accountPasswordSet = value; }
    }
    
    //End getters and setters----------------------------------------
}
