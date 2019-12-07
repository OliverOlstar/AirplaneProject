using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decision : MonoBehaviour
{
    private IState[] _states;
    private IState _currentState;

    [SerializeField] private Transform _target;

    void Start()
    {
        //Get States
        _states = GetComponents<IState>();
        PlaneController controller = GetComponentInChildren<PlaneController>();
        PlanePhysics physics = GetComponentInChildren<PlanePhysics>();

        //Setup States
        foreach(IState state in _states)
        {
            state.Setup(_target, controller, physics);
        }

        //Start on least priority State that can be entered
        StartLastState();
    }

    private void StartLastState()
    {
        //Enter least priority State that can be entered
        for (int i = _states.Length - 1; i >= 0; i--)
        {
            //Get distance to target
            float distance = Vector3.Distance(transform.position, _target.position);

            //Check if can Enter
            if (_states[i].CanEnter(distance))
            {
                _currentState = _states[i];
                _currentState.Enter();
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        CheckStates();
    }

    private void Update()
    {
        _currentState.Tick();
    }

    private void CheckStates()
    {
        //Get distance to target
        float distance = Vector3.Distance(transform.position, _target.position);

        //Return if you can't Exit current state
        if (_currentState.CanExit(distance) == false) return;

        foreach (IState state in _states)
        {
            //Check if can stay in same state
            if (_currentState == state)
            {
                if (state.CanEnter(distance))
                    break;
                else
                    continue;
            }

            //Check if state can be entered
            if (state.CanEnter(distance))
            {
                SwitchState(state);
                break;
            }
        }
    }

    //Exit old state and Enter new state
    private void SwitchState(IState pState)
    {
        _currentState.Exit();
        pState.Enter();
        _currentState = pState;
    }

    // Public Functions //////////////////////////////
    //Called when needing to switch to a state that is not a state the AI wants to be in (stunned, hurt, etc.)
    public void ForceStateSwitch(IState pState)
    {
        SwitchState(pState);
    }

    //Resets the AI for respawning
    public void Respawn()
    {
        _currentState.Exit();
        StartLastState();
    }
}
