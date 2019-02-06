using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class ControllerInput {

  public enum Name { X, LeftStickHorizontal, LeftStickVertical };

  [SerializeField]
  private UnityEvent onEvent;

  [SerializeField]
  private UnityEvent offEvent;

  [SerializeField]
  private UnityEvent turnOnEvent;

  [SerializeField]
  private UnityEvent turnOffEvent;

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

  public PositiveDirectionInput(Name name) : base(name) {}

  public override bool IsOn(string axisName) {
    return Input.GetAxis(axisName) > 0;
  }

}

[System.Serializable]
public class NegativeDirectionInput : ControllerInput {

  public NegativeDirectionInput(Name name) : base(name) {}

  public override bool IsOn(string axisName) {
    return Input.GetAxis(axisName) < 0;
  }

}