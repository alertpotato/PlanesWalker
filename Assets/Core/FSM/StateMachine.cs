using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public StateBehaviour CurrentState
    {
        get { return _currentState; }
        set
        {
            _currentState.OnExit();
            _currentState = value;
            _currentState.SetMachine(this);
            _currentState.OnEnter();
        }
    }

    [SerializeField]
    private StateBehaviour _currentState;

    void Start()
    {
        if (_currentState != null)
        {
            _currentState.SetMachine(this);
            _currentState.OnEnter();
        }
    }

    void Update()
    {
        CurrentState.OnUpdate();
    }

    void FixedUpdate()
    {
        CurrentState.OnFixedUpdate();
    }


    public void ChangeState(StateBehaviour newState)
    {
        CurrentState = newState;
    }

    public void ChangeState<T>() where T : StateBehaviour
    {
        ChangeState(GetComponent<T>());
    }
}
