using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateEnemy<T> : IState
{
    protected T Fsm;

    public BaseStateEnemy(T fsm)
    {
        Fsm = fsm;
    }
    public abstract void Enter();
    public abstract void UpdateState();
    public abstract void Exit();
}
