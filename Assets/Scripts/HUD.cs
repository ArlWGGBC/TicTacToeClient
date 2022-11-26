using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static TMPro.TextMeshPro;

public class HUD : MonoBehaviour
{
    [SerializeField] private NetworkedClient _client;
    
    [SerializeField] private TextMeshProUGUI displayText;
    
    [SerializeField] private TextMeshProUGUI displayTextLogin;
    
    [SerializeField] private TextMeshProUGUI playersOnline;

    [SerializeField] public Transform playerListParent;
    
    [SerializeField] public Transform textboxListParent;
    
    [SerializeField] public Slot[] PlayerNameSlots;

    
    [SerializeField] public Slot[] MessageSlots;
    
    [SerializeField] private TextMeshProUGUI roomName;

    [SerializeField] private TMP_InputField _textbox;
    
    [SerializeField] private TMP_InputField roomInputField;
    
    [SerializeField] private TMP_InputField nameInputField;

    [SerializeField] private TMP_InputField passwordInputField;
    
    [SerializeField] private TMP_InputField nameLoginInputField;

    [SerializeField] private TMP_InputField passwordLoginInputField;

    [SerializeField] private GameObject createAccountPanel;

    [SerializeField] private GameObject startPanel;
    
    [SerializeField] private GameObject loginPanel;

    [SerializeField] private GameObject loggedInPanel;
    
    [SerializeField] private GameObject gameRoomPanel;
    


    public List<Slot> textBox;
    public List<Slot> playerNameBox;
    public List<Slot> miscBox;

    public List<string> roomnames;
    
    private int numberOnline = 0;
    // Start is called before the first frame update
    void Start()
    {
        PlayerNameSlots = playerListParent.GetComponentsInChildren<Slot>();
        MessageSlots = textboxListParent.GetComponentsInChildren<Slot>();

        foreach (var Slot in PlayerNameSlots)
        {
            playerNameBox.Add(Slot);
        }

        foreach (var messageSlot in MessageSlots)
        {
            textBox.Add(messageSlot);
        }
    }

    public void PopulateRoomNames(List<string> id)
    {
        if (id.Count <= 0)
            return;
        else
        {
            foreach (var player in playerNameBox)
            {
                player.SetName("");
                player.isFilled = false;
            }
        }

        foreach (var player in id)
        {
            foreach (var slot in playerNameBox)
            {
                if (!slot.isFilled)
                {
                    slot.SetName("Player : " + player);
                    slot.isFilled = true;
                    break;
                }
            }
        }
        
    }

    public bool CheckMessage()
    {

        if (_textbox.text.Contains("poo"))
        {
            SendErrorMessage();
            return false;
        }
        
        return true;
    }

    private void SendErrorMessage()
    {
        foreach (var slot in textBox)
        {
            if (!slot.isFilled)
            {
                slot.SetName("You cant say : " + _textbox.text);
                slot.isFilled = true;
                break;
            }
        }
        
        foreach (var player in textBox)
        {
            player.SetName("");
            player.isFilled = false;
        }
    }

    public void AddChatMessage(string text)
    {
        foreach (var slot in textBox)
        {
                if (!slot.isFilled)
                {
                    slot.SetName(text);
                    slot.isFilled = true;
                    return;
                }
        }


        foreach (var slot in textBox)
        {
            slot.SetName("");
            slot.isFilled = false;
        }
        
        
    }
    
    
    public void RemoveRoomName(string id)
    {
        foreach (var player in PlayerNameSlots)
        {

            if (player.slotName.text == id)
            {
                player.SetName("");
                player.isFilled = false;
            }
            
        }
        
    }
    
    public void ResetRoomHUD()
    {
        foreach (var player in PlayerNameSlots)
        {
            player.SetName("");
            player.isFilled = false;
        }

        roomName.text = "";

    }
    
    
    public void SetUIText(string text)
    {
        displayText.text = text;
    }
    
    public void SetUITextLogin(string text)
    {
        displayTextLogin.text = text;
    }
    
    public string GetNameInput()
    {
        return nameInputField.text;
    }

    public void SetRoomName(string text)
    {
        roomName.text = text;
    }
    
    public string GetRoomInput()
    {
        return roomInputField.text;
    }

    public string GetTextBoxInput()
    {
        return _textbox.text;
    }

    public string GetPasswordInput()
    {
        return passwordInputField.text;
    }
    
    public string GetLoginNameInput()
    {
        return nameLoginInputField.text;
    }

    public string GetLoginPasswordInput()
    {
        return passwordLoginInputField.text;
    }

    public void SwitchCreateAccountScreen()
    {
        startPanel.SetActive(false);
        createAccountPanel.SetActive(true);
        
    }
    
    public void SwitchLogInScreen()
    {
        startPanel.SetActive(false);
        createAccountPanel.SetActive(false);
        loginPanel.SetActive(true);
        
    }
    
    public void SwitchGameRoomScreen()
    {
        startPanel.SetActive(false);
        createAccountPanel.SetActive(false);
        loginPanel.SetActive(false);
        loggedInPanel.SetActive(false);
        gameRoomPanel.SetActive(true);
        
        

    }

    public void OnLoggedInScreen()
    {
        startPanel.SetActive(false);
        createAccountPanel.SetActive(false);
        loginPanel.SetActive(false);
        gameRoomPanel.SetActive(false);
        loggedInPanel.SetActive(true);
    }

    public void UpdatePlayersOnlineCount(int player)
    {
        
        playersOnline.text = ("Players Online : " + player);
    }

    public void UpdatePlayerListCount(int player1ID, int player2ID)
    {
        
    }
    

    public void UserAlreadyExists()
    {
        
    }

    public void SwitchStartScreen()
    {
        startPanel.SetActive(true);
        createAccountPanel.SetActive(false);
        loginPanel.SetActive(false);
        gameRoomPanel.SetActive(false);
        loggedInPanel.SetActive(false);
        
        
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
