using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public TextMeshProUGUI slotName;

    [SerializeField]
    public bool isFilled = false;

    public bool isPlayerSlot;
    public bool isTextSlot;
    public bool isMiscSlot;
   

    public void SetName(string names)
    {
        slotName.text = names;
       // Debug.Log(slotName.text);
    }
   
}
