using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class ControllerInput
{

  public enum JoystickFamily
  {
    XBox,
    Playstation
  }

  public enum Name
  {
    LeftStickHorizontal,
    LeftStickVertical,
    XBox_DPadHorizontal,
    XBox_DPadVertical,
    XBox_A,
    PS4_DPadHorizontal,
    PS4_DPadVertical,
    PS4_X,
  };

  [SerializeField]
  private UnityEvent onEvent = new UnityEvent();

  public UnityEvent On
  {
    get
    {
      return onEvent;
    }
  }

  [SerializeField]
  private UnityEvent offEvent = new UnityEvent();

  public UnityEvent Off
  {
    get
    {
      return offEvent;
    }
  }

  [SerializeField]
  private UnityEvent turnOnEvent = new UnityEvent();

  public UnityEvent TurnOn
  {
    get
    {
      return turnOnEvent;
    }
  }

  [SerializeField]
  private UnityEvent turnOffEvent = new UnityEvent();

  public UnityEvent TurnOff
  {
    get
    {
      return turnOffEvent;
    }
  }

  protected Name name;
  private bool on;

  public ControllerInput(Name name)
  {
    this.name = name;
  }

  public ControllerInput() { }

  public abstract bool IsOn(int controllerIndex);

  public void Update(int controllerIndex)
  {
    if (IsOn(controllerIndex))
    {
      if (!on)
      {
        turnOnEvent.Invoke();
      }

      onEvent.Invoke();
      on = true;
    }
    else
    {
      if (on)
      {
        turnOffEvent.Invoke();
      }

      offEvent.Invoke();
      on = false;
    }
  }
}

[System.Serializable]
public class ControllerButton : ControllerInput
{

  public ControllerButton(Name name) : base(name) { }

  public override bool IsOn(int controllerIndex)
  {
    string axisName = ControllersManager.Instance.GetAxisName(controllerIndex, name);

    return Input.GetButton(axisName);
  }

}

[System.Serializable]
public class PositiveDirectionInput : ControllerInput
{

  private float threshold;

  public PositiveDirectionInput(Name name, float threshold = 0.9f) : base(name)
  {
    this.threshold = threshold;
  }

  public override bool IsOn(int controllerIndex)
  {
    string axisName = ControllersManager.Instance.GetAxisName(controllerIndex, name);

    return Input.GetAxis(axisName) > threshold;
  }

}

[System.Serializable]
public class NegativeDirectionInput : ControllerInput
{

  private float threshold;

  public NegativeDirectionInput(Name name, float threshold = -0.9f) : base(name)
  {
    this.threshold = threshold;
  }

  public override bool IsOn(int controllerIndex)
  {
    string axisName = ControllersManager.Instance.GetAxisName(controllerIndex, name);

    return Input.GetAxis(axisName) < threshold;
  }

}

[System.Serializable]
public class DeadInput : ControllerInput
{
  public override bool IsOn(int controllerIndex)
  {
    return false;
  }
}

[System.Serializable]
public class JoystickFamilySwitch : ControllerInput
{

  private ControllerInput mappedInput;
  private Dictionary<JoystickFamily, ControllerInput> familyMap;

  public JoystickFamilySwitch(Dictionary<JoystickFamily, ControllerInput> familyMap)
  {
    this.familyMap = familyMap;
  }

  public override bool IsOn(int controllerIndex)
  {
    if (mappedInput == null)
    {
      var family = ControllersManager.Instance.GetControllerJoystickFamily(controllerIndex);

      if (familyMap.ContainsKey(family))
      {
        mappedInput = familyMap[family];
      }
      else
      {
        mappedInput = new DeadInput();
      }
    }

    return mappedInput.IsOn(controllerIndex);
  }

}
