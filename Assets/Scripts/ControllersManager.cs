using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ControllersManager {

    private const string PS4_CONTROLLER_NAME = "Wireless Controller";

    private IEnumerable<int> joystickIndexes = Enumerable.Range(1, 16);
    private IEnumerable<int> controllerIndexes = Enumerable.Range(1, 2);

    private Dictionary<ControllerInput.Name, Dictionary<int, string>> axesNames = new Dictionary<ControllerInput.Name, Dictionary<int, string>>();

    private Dictionary<int, int> links = new Dictionary<int, int>();

    Dictionary<int, string> GenerateInputAxesNames(string format) {
        return joystickIndexes.ToDictionary(i => i, i => string.Format(format, i));
    }

    private int lastUpdateFrame;

    private static ControllersManager instance;

    public static ControllersManager Instance {
        get {
            if (instance == null) {
                instance = new ControllersManager();
            }

            return instance;
        }
    }

    private ControllersManager() {
        axesNames[ControllerInput.Name.X] = GenerateInputAxesNames("ARW_X_Joystick{0}");
        axesNames[ControllerInput.Name.LeftStickHorizontal] = GenerateInputAxesNames("ARW_LeftStickHorizontal_Joystick{0}");
        axesNames[ControllerInput.Name.LeftStickVertical] = GenerateInputAxesNames("ARW_LeftStickVertical_Joystick{0}");
        axesNames[ControllerInput.Name.XBoxDPadHorizontal] = GenerateInputAxesNames("ARW_XBoxDPadHorizontal_Joystick{0}");
        axesNames[ControllerInput.Name.XBoxDPadVertical] = GenerateInputAxesNames("ARW_XBoxDPadVertical_Joystick{0}");
        axesNames[ControllerInput.Name.PS4DPadHorizontal] = GenerateInputAxesNames("ARW_PS4DPadHorizontal_Joystick{0}");
        axesNames[ControllerInput.Name.PS4DPadVertical] = GenerateInputAxesNames("ARW_PS4DPadVertical_Joystick{0}");
    }

    public ControllerInput.JoystickFamily? GetJoystickFamily(int joystickIndex) {
        var joystickNames = Input.GetJoystickNames();
        var enumerableIndex = joystickIndex - 1;

        if (enumerableIndex >= joystickNames.Count()) {
            Debug.LogError(string.Format("Joystick index {0} is out of bounds", joystickIndex));
            return null;
        }

        return joystickNames[enumerableIndex] == PS4_CONTROLLER_NAME
            ? ControllerInput.JoystickFamily.Playstation
            : ControllerInput.JoystickFamily.XBox;
    }

    public ControllerInput.JoystickFamily GetControllerJoystickFamily(int controllerIndex) {
        int joystickIndex = links[controllerIndex];

        return GetJoystickFamily(joystickIndex).Value;
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
        var joystickFamily = GetJoystickFamily(joystickIndex);

        if (!IsJoystickLinked(joystickIndex) && joystickFamily.HasValue) {
            int? controllerIndex = GetFirstUnlinkedController();

            if (controllerIndex.HasValue) {
                Debug.Log(string.Format("Controller {0} has been linked to Joystick {1} ({2} family)", controllerIndex, joystickIndex, joystickFamily.Value));
                links[controllerIndex.Value] = joystickIndex;
            }
        }
    }

    bool IsJoystickXPressed(int joystickIndex) {
        return Input.GetAxis(axesNames[ControllerInput.Name.X][joystickIndex]) == 1;
    }

    void CheckAndLinkJoystick(int joystickIndex) {
        if (IsJoystickXPressed(joystickIndex)) {
            LinkJoystick(joystickIndex);
        }
    }

    public void Update() {
        if (lastUpdateFrame != Time.frameCount) {
            if (HasUnlinkedController()) {
                foreach(int joystickIndex in joystickIndexes) {
                    CheckAndLinkJoystick(joystickIndex);
                }
            }

            lastUpdateFrame = Time.frameCount;
        }
    }

    public bool isControllerLinked(int controllerIndex) {
        return links.ContainsKey(controllerIndex);
    }

    public string GetAxisName(int controllerIndex, ControllerInput.Name name) {
        if (links.ContainsKey(controllerIndex)) {
            int joystickIndex = links[controllerIndex];

            return axesNames[name][joystickIndex];
        } else {
            return null;
        }
    }

}