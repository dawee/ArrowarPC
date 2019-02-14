using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour {

    enum State {Initial, Linked, Ready};

    [SerializeField]
    public int Index;

    [SerializeField]
    UnityEvent readyEvent = new UnityEvent();

    public UnityEvent ReadyEvent {
        get {
            return readyEvent;
        }
    }

    private State state = State.Initial;

    [SerializeField]
    private ControllerButton xInput = new ControllerButton(ControllerInput.Name.X);

    public ControllerButton XInput {
        get {
            return xInput;
        }
    }

    [SerializeField]
    private NegativeDirectionInput leftInput = new NegativeDirectionInput(ControllerInput.Name.DPadHorizontal);

    public NegativeDirectionInput LeftInput {
        get {
            return leftInput;
        }
    }

    [SerializeField]
    private PositiveDirectionInput rightInput = new PositiveDirectionInput(ControllerInput.Name.DPadHorizontal);

    public PositiveDirectionInput RightInput {
        get {
            return rightInput;
        }
    }

    [SerializeField]
    private NegativeDirectionInput downInput = new NegativeDirectionInput(ControllerInput.Name.DPadVertical);

    public NegativeDirectionInput DownInput {
        get {
            return downInput;
        }
    }

    [SerializeField]
    private PositiveDirectionInput upInput = new PositiveDirectionInput(ControllerInput.Name.DPadVertical);

    public PositiveDirectionInput UpInput {
        get {
            return upInput;
        }
    }

    void Update() {
        ControllersManager.Instance.Update();
        
        switch (state) {
            case State.Initial:
                if (ControllersManager.Instance.isControllerLinked(Index)) {
                    state = State.Linked;
                }
                break;
            case State.Linked:
                if (!XInput.IsOn(ControllersManager.Instance, Index)) {
                    state = State.Ready;
                    readyEvent.Invoke();
                }
                break;
            case State.Ready:
                XInput.Update(ControllersManager.Instance, Index);
                LeftInput.Update(ControllersManager.Instance, Index);
                RightInput.Update(ControllersManager.Instance, Index);
                UpInput.Update(ControllersManager.Instance, Index);
                DownInput.Update(ControllersManager.Instance, Index);
                break;
            default:
                break;            
        }
    }

}