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

    [SerializeField] private TMP_InputField nameInputField;

    [SerializeField] private TMP_InputField passwordInputField;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetUIText(string text)
    {
        displayText.text = text;
    }
    
    public string GetNameInput()
    {
        return nameInputField.text;
    }

    public string GetPasswordInput()
    {
        return passwordInputField.text;
    }
    void Initialize()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
