using System.Collections;


public abstract class StateMachine
{
    protected readonly NetworkedClient _system;


  
    public StateMachine(NetworkedClient system)
    {
        _system = system;
    }
        
    public virtual void Start()
    {
        
        return;
    }
    

    public virtual IEnumerator Pause()
    {
        yield break;
    }

    public virtual IEnumerator Resume()
    {
        yield break;
    }

    public virtual IEnumerator SetName()
    {
        yield break;
    }
    
    public virtual IEnumerator SetPassword()
    {
        yield break;
    }

    public virtual IEnumerator CheckAccount()
    {
        yield break;
    }
    

}
