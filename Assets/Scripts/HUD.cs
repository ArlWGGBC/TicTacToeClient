using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static TMPro.TextMeshPro;

public class HUD : MonoBehaviour
{

    #region References
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
    
    [SerializeField] public TMP_InputField nameInputField;

    [SerializeField] public TMP_InputField passwordInputField;
    
    [SerializeField] public TMP_InputField nameLoginInputField;

    [SerializeField] public TMP_InputField passwordLoginInputField;

    [SerializeField] private GameObject createAccountPanel;

    [SerializeField] private GameObject startPanel;
    
    [SerializeField] private GameObject loginPanel;

    [SerializeField] private GameObject loggedInPanel;
    
    [SerializeField] private GameObject gameRoomPanel;

    [SerializeField] private GameObject replayPanel;
    
    [SerializeField] private CustomDropdown replayDropdown;

    [SerializeField] private Sprite icon;
    
    
    
    
    public Transform Replay_BoardParent;
    public BoardTile[] Replay_TTT_Tiles;

    //Can transition to HUD -> Possibly wrap this in a separate class -> init the TTT_Tiles with constructor and hold a reference to it in HUD class.
    public Transform BoardParent;
    public BoardTile[] TTT_Tiles;

    #endregion
    
    


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
        
        
        
        TTT_Tiles = BoardParent.GetComponentsInChildren<BoardTile>();
        Replay_TTT_Tiles = Replay_BoardParent.GetComponentsInChildren<BoardTile>();
        
        for (int i = 0; i < TTT_Tiles.Length; i++)
        {
            TTT_Tiles[i].boardPosition = i + 1;
            Replay_TTT_Tiles[i].boardPosition = i + 1;
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


    public void AddReplay(string title)
    {
        replayDropdown.CreateNewItem(title, icon, true);
        
        replayDropdown.SetupDropdown();
        
        replayDropdown.onValueChanged.AddListener(PopulateReplay);

    }

    private void PopulateReplay(int value)
    {
        StopAllCoroutines();
        ResetReplayBoard();
        
       string replayName =  replayDropdown.items[value].itemName;

       foreach (var replay in _client._Processing.replays)
       {
           if (replayName != replay.roomname) continue;

           StartCoroutine(ReplayMove(replay));
       }
       
    }
    
    
    IEnumerator ReplayMove(Replay replay)
    {
        foreach (var move in replay.moves)
        {
            yield return new WaitForSeconds(1f);
            
            foreach (var tile in Replay_TTT_Tiles)
            {
                if (Convert.ToInt32(move.pos) == tile.boardPosition)
                {
                    tile.SetTile(move.identifier);
                }
            }
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

        foreach (var tiles in TTT_Tiles)
        {
            tiles.SetTile(identifier.N.ToString());
            tiles.isBlank = true;
        }

        roomName.text = "";

    }

    public void ResetTicTacBoard()
    {
        foreach (var tiles in TTT_Tiles)
        {
            tiles.SetTile(identifier.N.ToString());
            tiles.isBlank = true;
        }
    }
    
    public void ResetReplayBoard()
    {
        foreach (var tiles in Replay_TTT_Tiles)
        {
            tiles.SetTile(identifier.N.ToString());
            tiles.isBlank = true;
        }
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
    
    
  

    public void SwitchReplayScreen()
    {
        startPanel.SetActive(false);
        createAccountPanel.SetActive(false);
        loginPanel.SetActive(false);
        loggedInPanel.SetActive(false);
        replayPanel.SetActive(true);
        _client.SendMessageToHost(Messages.GetReplays);
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
        replayPanel.SetActive(false);
        loggedInPanel.SetActive(false);
        
        
        
    }

    public void SwitchLoggedInScreen()
    {
        startPanel.SetActive(false);
        createAccountPanel.SetActive(false);
        loginPanel.SetActive(false);
        gameRoomPanel.SetActive(false);
        replayPanel.SetActive(false);
        loggedInPanel.SetActive(true);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
