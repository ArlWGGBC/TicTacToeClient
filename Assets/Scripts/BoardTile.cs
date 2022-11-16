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
        _positionID = identifier.BLANK;
       

        client = FindObjectOfType<NetworkedClient>();
    }

    public void MakeMove()
    {
        SetTile(client.isO);
    }
    
    private void SetTile(bool isO)
    {
        if (isO)
            _image.sprite = o;
        else
        { 
            _image.sprite = x;
        }
    }
}

public enum identifier
{
    X,
    O,
    BLANK
}