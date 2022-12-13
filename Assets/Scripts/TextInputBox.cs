using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInputBox : MonoBehaviour
{
    private HUD hud;

    private NetworkedClient _client;


    private bool canSendMessage = true;
    // Start is called before the first frame update
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        _client = FindObjectOfType<NetworkedClient>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return) && canSendMessage)
            {
                if (!hud.CheckMessage())
                    return;
                
                Debug.Log("Player Sent Message");
                _client.SendMessageToHost(Messages.MessageC + "," +_client._currentRoom  + "," + hud.GetTextBoxInput() + "," + _client.ConnectionID);
                canSendMessage = false;
                StartCoroutine(MessageDelay());
            }
                
        }
    }


    IEnumerator MessageDelay()
    {
        yield return new WaitForSeconds(0.25f);
        canSendMessage = true;

    }
}
