using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSlot : MonoBehaviour
{
    public TextMeshProUGUI slotName;

    public bool isFilled = false;

    public void SetName(string names)
    {
        slotName.text = names;
       // Debug.Log(slotName.text);
    }
   
}
