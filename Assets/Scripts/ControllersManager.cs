using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllersManager : MonoBehaviour {

  public enum InputName { X_BUTTON };

  private IEnumerable<int> joystickIndexes = Enumerable.Range(1, 16);
  private IEnumerable<int> controllerIndexes = Enumerable.Range(1, 2);

  private Dictionary<InputName, Dictionary<int, string>> axesNames = new Dictionary<InputName, Dictionary<int, string>>();

  private Dictionary<int, int> links = new Dictionary<int, int>();

  Dictionary<int, string> GenerateInputAxesNames(string format) {
    return joystickIndexes.ToDictionary(i => i, i => string.Format(format, i));
  }

  void Awake() {
    axesNames[InputName.X_BUTTON] = GenerateInputAxesNames("ARW_X_Joystick{0}");
  }

  bool IsJoystickLinked(int joystickIndex) {
    return links.ContainsValue(joystickIndex);
  }

  bool HasUnlinkedController() {
    return links.Count < controllerIndexes.Count();
  }

  int? GetFirstUnlinkedController() {
    if (HasUnlinkedController()) {
      return controllerIndexes.First(
        controllerIndex => !links.ContainsKey(controllerIndex)
      );
    } else {
      return null;
    }
  }

  void LinkJoystick(int joystickIndex) {
    if (!IsJoystickLinked(joystickIndex)) {
      int? controllerIndex = GetFirstUnlinkedController();

      if (controllerIndex.HasValue) {
        Debug.Log(string.Format("Controller {0} has been linked to Joystick {1}", controllerIndex, joystickIndex));
        links[controllerIndex.Value] = joystickIndex;
      }
    }
  }

  bool IsJoystickXPressed(int joystickIndex) {
    return Input.GetAxis(axesNames[InputName.X_BUTTON][joystickIndex]) == 1;
  }

  void CheckAndLinkJoystick(int joystickIndex) {
    if (IsJoystickXPressed(joystickIndex)) {
      LinkJoystick(joystickIndex);
    }
  }

  void Update() {
    if (HasUnlinkedController()) {
      foreach(int joystickIndex in joystickIndexes) {
        CheckAndLinkJoystick(joystickIndex);
      }
    }
  }

  public bool isControllerLinked(int controllerIndex) {
    return links.ContainsKey(controllerIndex);
  }

}