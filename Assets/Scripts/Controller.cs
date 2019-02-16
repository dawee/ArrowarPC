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
    private JoystickFamilySwitch leftInput = new JoystickFamilySwitch(
        new Dictionary<ControllerInput.JoystickFamily, ControllerInput> () {
            {ControllerInput.JoystickFamily.Playstation, new NegativeDirectionInput(ControllerInput.Name.PS4DPadHorizontal)},
            {ControllerInput.JoystickFamily.XBox, new NegativeDirectionInput(ControllerInput.Name.XBoxDPadHorizontal)},
        }
        

    );

    public JoystickFamilySwitch LeftInput {
        get {
            return leftInput;
        }
    }

    [SerializeField]
    private JoystickFamilySwitch rightInput = new JoystickFamilySwitch(
        new Dictionary<ControllerInput.JoystickFamily, ControllerInput> () {
            {ControllerInput.JoystickFamily.Playstation, new PositiveDirectionInput(ControllerInput.Name.PS4DPadHorizontal)},
            {ControllerInput.JoystickFamily.XBox, new PositiveDirectionInput(ControllerInput.Name.XBoxDPadHorizontal)},
        }
        

    );

    public JoystickFamilySwitch RightInput {
        get {
            return rightInput;
        }
    }

    [SerializeField]
    private JoystickFamilySwitch downInput = new JoystickFamilySwitch(
        new Dictionary<ControllerInput.JoystickFamily, ControllerInput> () {
            {ControllerInput.JoystickFamily.Playstation, new NegativeDirectionInput(ControllerInput.Name.PS4DPadVertical)},
            {ControllerInput.JoystickFamily.XBox, new NegativeDirectionInput(ControllerInput.Name.XBoxDPadVertical)},
        }
        

    );

    public JoystickFamilySwitch DownInput {
        get {
            return downInput;
        }
    }

    [SerializeField]
    private JoystickFamilySwitch upInput = new JoystickFamilySwitch(
        new Dictionary<ControllerInput.JoystickFamily, ControllerInput> () {
            {ControllerInput.JoystickFamily.Playstation, new PositiveDirectionInput(ControllerInput.Name.PS4DPadVertical)},
            {ControllerInput.JoystickFamily.XBox, new PositiveDirectionInput(ControllerInput.Name.XBoxDPadVertical)},
        }
    );

    public JoystickFamilySwitch UpInput {
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
                if (!XInput.IsOn(Index)) {
                    state = State.Ready;
                    readyEvent.Invoke();
                }
                break;
            case State.Ready:
                XInput.Update(Index);
                LeftInput.Update(Index);
                RightInput.Update(Index);
                UpInput.Update(Index);
                DownInput.Update(Index);
                break;
            default:
                break;            
        }
    }

}