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

    

    #endregion
    
    #region Class References
    
    public StateMachine _currentState;
    
    public MessageProcessing _Processing;
    
    [SerializeField] public HUD hud;
    
    private PlayerData _playerData;
    #endregion
    

    //Current account credentials local
    protected string accountName;
    
    //Is connection id
    protected int accountID;

    //roomName local
    public string roomName;

    
    public void SetState(StateMachine state)
    {

        _currentState = state;
        _currentState.Start();
    }
//----------------------------------------------------------


    #region Unity Functions
    
    void Start()
    {

        _playerData = GetComponentInChildren<PlayerData>();
        
        if (_playerData == null)
        {
            this.AddComponent<PlayerData>();
            _playerData = GetComponentInChildren<PlayerData>();
        }
        
        _Processing = GetComponentInChildren<MessageProcessing>();

        if (_Processing == null)
        {
            this.AddComponent<MessageProcessing>();
            _Processing = GetComponentInChildren<MessageProcessing>();
        }
 
        Connect();
        SetState(new CreateAccountState(this));
        
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
                NetworkTransport.Connect(hostID, "192.168.2.11", socketPort, 0,
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

        _Processing.ProcessMessage(msg, id);
        
    }

    

    #endregion


    #region Getters and Setters
    
    public bool IsConnected()
    {
        return isConnected;
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
    


    #endregion
    
    
    

}
