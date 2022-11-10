using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConnectedState : StateMachine
{
    
    
    public  ConnectedState(NetworkedClient system) : base(system)
    {
        
    }
    // Start is called before the first frame update
    public override void Start()
    {
        
        Debug.Log("Connected");
        return;


    }

    // Update is called once per frame
    void Update()
    {
       
    }
    
    
    
    
}
