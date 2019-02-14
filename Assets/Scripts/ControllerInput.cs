using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class ControllerInput {

    public enum Name {
        X,
        LeftStickHorizontal,
        LeftStickVertical,
        DPadHorizontal,
        DPadVertical,
    };

    [SerializeField]
    private UnityEvent onEvent = new UnityEvent();

    public UnityEvent OnEvent {
        get {
            return onEvent;
        }
    }

    [SerializeField]
    private UnityEvent offEvent = new UnityEvent();

    public UnityEvent OffEvent {
        get {
            return offEvent;
        }
    }

    [SerializeField]
    private UnityEvent turnOnEvent = new UnityEvent();

    public UnityEvent TurnOnEvent {
        get {
            return turnOnEvent;
        }
    }

    [SerializeField]
    private UnityEvent turnOffEvent = new UnityEvent();

    public UnityEvent TurnOffEvent {
        get {
            return turnOffEvent;
        }
    }

    private Name name;
    private bool on;

    public ControllerInput(Name name) {
        this.name = name;
    }

    public abstract bool IsOn(string axisName);

    public bool IsOn(ControllersManager manager, int controllerIndex) {
        string axisName = manager.GetAxisName(controllerIndex, name);

        return IsOn(axisName);
    }

    public void Update(ControllersManager manager, int controllerIndex) {
        if (IsOn(manager, controllerIndex)) {
            if (!on) {
                turnOnEvent.Invoke();
            }

            onEvent.Invoke();
            on = true;
        } else {
            if (on) {
                turnOffEvent.Invoke();
            }

            offEvent.Invoke();
            on = false;
        }
    }
}

[System.Serializable]
public class ControllerButton : ControllerInput {

    public ControllerButton(Name name) : base(name) {}

    public override bool IsOn(string axisName) {
        return Input.GetButton(axisName);
    }

}

[System.Serializable]
public class PositiveDirectionInput : ControllerInput {

    private float threshold;

    public PositiveDirectionInput(Name name, float threshold = 0.9f) : base(name) {
        this.threshold = threshold;
    }

    public override bool IsOn(string axisName) {
        return Input.GetAxis(axisName) > threshold;
    }

}

[System.Serializable]
public class NegativeDirectionInput : ControllerInput {

    private float threshold;

    public NegativeDirectionInput(Name name, float threshold = -0.9f) : base(name) {
        this.threshold = threshold;
    }

    public override bool IsOn(string axisName) {
        return Input.GetAxis(axisName) < threshold;
    }

}