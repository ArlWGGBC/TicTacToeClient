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

    [SerializeField] private TextMeshProUGUI displayText;
    
    [SerializeField] private TextMeshProUGUI displayTextLogin;

    [SerializeField] private TextMeshProUGUI roomName;

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

  
    // Start is called before the first frame update
    void Start()
    {

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
        loggedInPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
