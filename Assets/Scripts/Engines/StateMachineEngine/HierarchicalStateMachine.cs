using System.Collections.Generic;

public class HierarchicalStateMachine
{
    private IState currentState;

    public void ChangeState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        // Enter hierarchy from top parent to child
        IState temp = newState;
        Stack<IState> enterStack = new Stack<IState>();
        while (temp != null)
        {
            enterStack.Push(temp);
            temp = temp.Parent;
        }
        while (enterStack.Count > 0)
            enterStack.Pop().Enter();
    }

    public void Update()
    {
        currentState?.Execute();
    }
}