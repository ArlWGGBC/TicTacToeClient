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
    
    private NetworkMatch m_NetworkMatch;
    
    private StateMachine _currentState;


    [SerializeField] public HUD hud;



    protected string accountName;
    protected string accountPassword;
    protected int accountID;

    protected string loginInputName;
    protected string loginInputPassword;

    protected bool accountNameSet = false;
    protected bool accountPasswordSet = false;

    protected bool canCreate = false;

    protected string roomName;

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
    
    
    public void SetState(StateMachine state)
    {
        
        _currentState = state;
        StartCoroutine(_currentState.Start());
    }

    
    // Start is called before the first frame update
    void Start()
    {
        Connect();
        SetState(new DisconnectedState(this));
        
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
                        //SetState(new ConnectedState(this));
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                        //SetState(new DisconnectedState(this));
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

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.73", socketPort, 0, out error); // server is local on network

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

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
    }

    public bool IsConnected()
    {
        return isConnected;
    }

    
    
    //Creating and Logging into account functionality + helper functions
    public void OnSubmitName()
    {
        StartCoroutine(_currentState.SetName());
    }
    
    public void OnSubmitPassword()
    {
        StartCoroutine(_currentState.SetPassword());
    }
    

    public void LoginName()
    {
        AccountNameInput = hud.GetLoginNameInput();
    }
    
    public void LoginPassword()
    {
        AccountPasswordInput = hud.GetLoginPasswordInput();
    }

    public void Check()
    {
        StartCoroutine(_currentState.CheckAccount());
    }

    public void CreateAccount()
    {
        if (!canCreate)
            return;
        
        
        hud.SetUIText("Account Created");
        string filePath = Application.persistentDataPath + "/account.sav";
        StreamWriter sw = new StreamWriter(filePath);
        Debug.Log("Account Created");
        Debug.Log("name" + "," + accountName + "password" + "," + accountPassword);
        sw.WriteLine("name" + "," + accountName);
        sw.WriteLine("password" + "," + accountPassword);
        sw.WriteLine("connectionID" + "," + connectionID);
        
        sw.Close();
        
        CreateLoginScreen();
    }

    public void LoginAccount()
    {
        bool nameCorrect = false;
        bool passwordCorrect = false;
        
        string saveFilePath = Application.persistentDataPath + "/account.sav";
        if (!File.Exists(saveFilePath))
            return;

        StreamReader sr = new StreamReader(saveFilePath);

        string line;

        while ((line = sr.ReadLine()) != null)
        {
            string[] stats = line.Split(',');
            Debug.Log(line);
            if (stats[0] == "name")
            {
                if (stats[1] == AccountNameInput)
                {
                    Debug.Log("name correct");
                    nameCorrect = true;
                }
            }
            else if (stats[0] == "password")
            {
                if (stats[1] == AccountPasswordInput)
                {
                    Debug.Log("password correct");
                    passwordCorrect = true;
                }
            }
        }

        if (nameCorrect && passwordCorrect)
        {
            SetState(new LoggedInState(this));
        }
        else
        {
            hud.SetUITextLogin("Incorrect Credentials");
        }
    }


    public void CreateAccountScreen()
    {
        hud.SwitchCreateAccountScreen();
    }
    
    public void CreateLoginScreen()
    {
        hud.SwitchLogInScreen();
    }

    public void SetRoomName()
    {
        hud.SetRoomName(hud.GetRoomInput());
        //m_NetworkMatch.CreateMatch(hud.GetRoomInput(), 4, true, "", "", "", 0, 0, null);
        hud.SwitchGameRoomScreen();
    }


}