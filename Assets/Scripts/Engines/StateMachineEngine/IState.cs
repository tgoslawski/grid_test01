using UnityEngine;

public interface IState
{
    void Enter();
    void Execute();
    void Exit();

    IState Parent { get; }
    void SetParent(IState parent);
}

public abstract class BaseState : IState
{
    public IState Parent { get; private set; }

    public void SetParent(IState parent)
    {
        Parent = parent;
    }

    public virtual void Enter()
    {
        Debug.Log($"Entering {GetType().Name}");
    }

    public virtual void Execute() { }

    public virtual void Exit()
    {
        Debug.Log($"Exiting {GetType().Name}");
    }
}