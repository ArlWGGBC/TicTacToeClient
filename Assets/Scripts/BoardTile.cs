using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BoardTile : MonoBehaviour
{

    [SerializeField]
    public Image _image;
    

    public Sprite x;
    public Sprite o;

    
    [SerializeField]
    public int boardPosition;

    public identifier _positionID;

    public bool isBlank;
    
    private NetworkedClient client;
    

    
    
    private void Start()
    {
        _image = GetComponent<Image>();
        isBlank = true;
        _positionID = identifier.N;
       

       
        client = FindObjectOfType<NetworkedClient>();
    }

    public void MakeMove()
    {
        Debug.Log("CLIENT IDENTITY : " + client.identity);
        client.SendMessageToHost(client._message.MakeMove + "," + client._currentRoom + "," + client.identity + "," + boardPosition);
    }
    
    public void SetTile(string identifier)
    {
        if (identifier == "O")
        {
            _image.sprite = o;
            _positionID = global::identifier.O;
        }
        else
        { 
            _image.sprite = x;
            _positionID = global::identifier.X;
        }
    }
}

public enum identifier
{
    X,
    O,
    N
}