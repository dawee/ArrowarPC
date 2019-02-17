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
    private JoystickFamilySwitch downButtonInput = new JoystickFamilySwitch(
        new Dictionary<ControllerInput.JoystickFamily, ControllerInput>() {
            {ControllerInput.JoystickFamily.Playstation, new ControllerButton(ControllerInput.Name.PS4_X)},
            {ControllerInput.JoystickFamily.XBox, new ControllerButton(ControllerInput.Name.XBox_A)},
        }
    );

    public JoystickFamilySwitch DownButton {
        get {
            return downButtonInput;
        }
    }

    [SerializeField]
    private JoystickFamilySwitch dPadLeftInput = new JoystickFamilySwitch(
        new Dictionary<ControllerInput.JoystickFamily, ControllerInput> () {
            {ControllerInput.JoystickFamily.Playstation, new NegativeDirectionInput(ControllerInput.Name.PS4_DPadHorizontal)},
            {ControllerInput.JoystickFamily.XBox, new NegativeDirectionInput(ControllerInput.Name.XBox_DPadHorizontal)},
        }
        

    );

    public JoystickFamilySwitch DPadLeft {
        get {
            return dPadLeftInput;
        }
    }

    [SerializeField]
    private JoystickFamilySwitch dPadRightInput = new JoystickFamilySwitch(
        new Dictionary<ControllerInput.JoystickFamily, ControllerInput> () {
            {ControllerInput.JoystickFamily.Playstation, new PositiveDirectionInput(ControllerInput.Name.PS4_DPadHorizontal)},
            {ControllerInput.JoystickFamily.XBox, new PositiveDirectionInput(ControllerInput.Name.XBox_DPadHorizontal)},
        }
        

    );

    public JoystickFamilySwitch DPadRight {
        get {
            return dPadRightInput;
        }
    }

    [SerializeField]
    private JoystickFamilySwitch dPadDownInput = new JoystickFamilySwitch(
        new Dictionary<ControllerInput.JoystickFamily, ControllerInput> () {
            {ControllerInput.JoystickFamily.Playstation, new NegativeDirectionInput(ControllerInput.Name.PS4_DPadVertical)},
            {ControllerInput.JoystickFamily.XBox, new NegativeDirectionInput(ControllerInput.Name.XBox_DPadVertical)},
        }
        

    );

    public JoystickFamilySwitch DPadDown {
        get {
            return dPadDownInput;
        }
    }

    [SerializeField]
    private JoystickFamilySwitch dPadUpInput = new JoystickFamilySwitch(
        new Dictionary<ControllerInput.JoystickFamily, ControllerInput> () {
            {ControllerInput.JoystickFamily.Playstation, new PositiveDirectionInput(ControllerInput.Name.PS4_DPadVertical)},
            {ControllerInput.JoystickFamily.XBox, new PositiveDirectionInput(ControllerInput.Name.XBox_DPadVertical)},
        }
    );

    public JoystickFamilySwitch DPadUp {
        get {
            return dPadUpInput;
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
                if (!DownButton.IsOn(Index)) {
                    state = State.Ready;
                    readyEvent.Invoke();
                }
                break;
            case State.Ready:
                DownButton.Update(Index);
                DPadLeft.Update(Index);
                DPadRight.Update(Index);
                DPadDown.Update(Index);
                DPadUp.Update(Index);
                break;
            default:
                break;            
        }
    }

}