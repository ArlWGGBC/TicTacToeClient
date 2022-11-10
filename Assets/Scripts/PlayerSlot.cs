using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSlot : MonoBehaviour
{
    public TextMeshProUGUI slotName;

    public bool isFilled = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetName(string names)
    {
        slotName.text = names;
        Debug.Log(slotName.text);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
