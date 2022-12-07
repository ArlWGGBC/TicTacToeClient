using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(NetworkedClient))]
public class PlayerData : MonoBehaviour
{
    private string Name;
    private string Password;


    private NetworkedClient _client;

    public void Awake()
    {
        _client = GetComponentInChildren<NetworkedClient>();
    }

    public string PlayerName
    {
        get => Name;
        set => Name = value;
    }

    public string PlayerPassword
    {
        get => Password;
        set => Password = value;
    }

    public void SetName()
    {
        if(_client.hud.nameInputField.IsActive())
        PlayerName = _client.hud.nameInputField.text;
        else
        {
            PlayerName = _client.hud.nameLoginInputField.text;
        }
    }
    public void SetPassword()
    {
        if (_client.hud.passwordInputField.IsActive())
        {
            PlayerPassword = _client.hud.passwordInputField.text;
        }
        else
        {
            PlayerPassword = _client.hud.passwordLoginInputField.text;
        }
    }
    
    public void CreateAccount()
    {
        Debug.Log(Name +", " + Password);
        if (String.IsNullOrEmpty(Name))
        {
            _client.hud.SetUIText("Both fields must be filled");
            return;
        }
        if (String.IsNullOrEmpty(Password))
        {
            _client.hud.SetUIText("Both fields must be filled");
            return;
        }
        
        _client.SendMessageToHost(_client.Messages.CreateAccount + "," + Name + "," + Password);
    }

    public void LoginAccount()
    {
        Debug.Log(Name +", " + Password);
        if (String.IsNullOrEmpty(Name))
        {
            _client.hud.SetUIText("Both fields must be filled");
            return;
        }
        if (String.IsNullOrEmpty(Password))
        {
            _client.hud.SetUIText("Both fields must be filled");
            return;
        }
        
        _client.SendMessageToHost(_client.Messages.LoginAccount + "," + Name + "," + Password);
    }
    
    
    
}
