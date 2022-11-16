using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{

    public string name;

    public BaseState(string name)
    {
        this.name = name;
    }

    // runs when entering the state
    public virtual void Enter() { }

    // runs on Update()
    public virtual void UpdateLogic() { }

    // runs when exiting the state
    public virtual void Exit() { }
}
