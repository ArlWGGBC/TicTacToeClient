using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoggedInState : StateMachine
{
    public  LoggedInState(NetworkedClient system) : base(system)
    {
    }

    public override IEnumerator Start()
    {
        _system.hud.SetUITextLogin("Logged In");
        yield return new WaitForSeconds(0.5f);
        _system.hud.OnLoggedInScreen();
        _system.AccountID = _system.ConnectionID;



        //Include UI state changes and any other changes related to starting game.
        
    }
    
    
}
