using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour {

  enum State {Initial, Linked, Ready};

  [SerializeField]
  private ControllersManager manager;

  [SerializeField]
  private int index;

  [SerializeField]
  UnityEvent readyEvent;

  private State state = State.Initial;

  [SerializeField]
  private ControllerButton XInput = new ControllerButton(ControllerInput.Name.X_BUTTON);

  void Update() {
    switch (state) {
      case State.Initial:
        if (manager.isControllerLinked(index)) {
          state = State.Linked;
        }
        break;
      case State.Linked:
        if (!XInput.IsOn(manager, index)) {
          state = State.Ready;
          readyEvent.Invoke();
        }
        break;
      case State.Ready:
        XInput.Update(manager, index);
        break;
      default:
        break;      
    }
  }

}