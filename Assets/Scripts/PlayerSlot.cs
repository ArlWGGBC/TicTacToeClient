using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSlot : MonoBehaviour
{
    private TextMeshProUGUI name;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetName(string names)
    {
        name.text = names;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
