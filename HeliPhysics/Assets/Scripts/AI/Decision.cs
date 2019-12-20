using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decision : MonoBehaviour
{
    private IState[] _states;
    private IState _currentState;
    private int _currentStateIndex;

    void Start()
    {
        //Get States
        _states = GetComponents<IState>();
        PlaneController controller = GetComponentInChildren<PlaneController>();
        PlanePhysics physics = GetComponentInChildren<PlanePhysics>();

        //Setup States
        foreach(IState state in _states)
        {
            state.Setup(controller, physics);
        }

        //Start on least priority State that can be entered
        StartFirstState();
    }

    private void StartFirstState()
    {
        _currentState = _states[0];
        _currentState.Enter();
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
        //Switch to next State when done current State
        if (_currentState.CanExit() == true)
        {
            _currentStateIndex++;
            SwitchState(_states[_currentStateIndex]);
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
}
