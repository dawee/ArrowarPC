using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour {

    enum State {Initial, Linked, Ready};

    [SerializeField]
    private ControllersManager manager;

    [SerializeField]
    public int Index;

    [SerializeField]
    UnityEvent readyEvent;

    private State state = State.Initial;

    [SerializeField]
    private ControllerButton xInput = new ControllerButton(ControllerInput.Name.X);

    public ControllerButton XInput {
        get {
            return xInput;
        }
    }

    [SerializeField]
    private NegativeDirectionInput leftInput = new NegativeDirectionInput(ControllerInput.Name.LeftStickHorizontal);

    public NegativeDirectionInput LeftInput {
        get {
            return leftInput;
        }
    }

    [SerializeField]
    private PositiveDirectionInput rightInput = new PositiveDirectionInput(ControllerInput.Name.LeftStickHorizontal);

    public PositiveDirectionInput RightInput {
        get {
            return rightInput;
        }
    }

    [SerializeField]
    private NegativeDirectionInput downInput = new NegativeDirectionInput(ControllerInput.Name.LeftStickVertical);

    public NegativeDirectionInput DownInput {
        get {
            return downInput;
        }
    }

    [SerializeField]
    private PositiveDirectionInput upInput = new PositiveDirectionInput(ControllerInput.Name.LeftStickVertical);

    public PositiveDirectionInput UpInput {
        get {
            return upInput;
        }
    }

    void Update() {
        switch (state) {
            case State.Initial:
                if (manager.isControllerLinked(Index)) {
                    state = State.Linked;
                }
                break;
            case State.Linked:
                if (!XInput.IsOn(manager, Index)) {
                    state = State.Ready;
                    readyEvent.Invoke();
                }
                break;
            case State.Ready:
                XInput.Update(manager, Index);
                LeftInput.Update(manager, Index);
                RightInput.Update(manager, Index);
                UpInput.Update(manager, Index);
                DownInput.Update(manager, Index);
                break;
            default:
                break;            
        }
    }

}