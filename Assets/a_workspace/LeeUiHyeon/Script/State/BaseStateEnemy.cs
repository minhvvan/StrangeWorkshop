using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BaseState클래스를 가져온 클래스. 
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
