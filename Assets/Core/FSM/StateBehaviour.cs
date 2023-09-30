using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public class StateBehaviour : MonoBehaviour
{
    private StateMachine Machine;

    public void SetMachine(StateMachine machine)
    {
        Machine = machine;
    }

    public void ChangeState(StateBehaviour newState)
    {
        Machine.ChangeState(newState);
    }

    public void ChangeState<T>() where T : StateBehaviour
    {
        Machine.ChangeState<T>();
    }

    public bool IsActive()
    {
        return Machine.CurrentState == this;
    }

    virtual public void OnEnter() { }
    virtual public void OnExit() { }
    virtual public void OnUpdate() { }
    virtual public void OnFixedUpdate() { }

}
