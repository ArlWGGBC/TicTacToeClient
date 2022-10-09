using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartState : StateMachine
{
    public StartState(NetworkedClient system) : base(system)
    {
    }

    public override IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        
    }
    
}
