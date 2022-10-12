using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.UI;
public class CreateAccountState : StateMachine
{
    public  CreateAccountState(NetworkedClient system) : base(system)
    {
    }

    public override IEnumerator Start()
    {
        _system.hud.SetUIText("Create an Account");
        Debug.Log("Test");
        yield return new WaitForSeconds(2f);
        
           
        //Include UI state changes and any other changes related to starting game.
        
    }


    public override IEnumerator SetName()
    {
        _system.AccountName = _system.hud.GetNameInput();
        _system.NameSet = true;
        yield return new WaitForSeconds(2f);
    }
    
    public override IEnumerator SetPassword()
    {
        _system.AccountPassword = _system.hud.GetPasswordInput();
        _system.PasswordSet = true;
        yield return new WaitForSeconds(2f);
    }
    

    public override IEnumerator CheckAccount()
    {

        if (!_system.NameSet || !_system.PasswordSet)
        {
            _system.hud.SetUIText("Need both account name and password");
            yield break;
        }

            _system.canCreateAccount = true;
            _system.CreateAccount();
        
            
        yield return new WaitForSeconds(2f);
    }




}
