using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour {

  [SerializeField]
  private ControllersManager manager;

  [SerializeField]
  private int index;

  [SerializeField]
  UnityEvent readyEvent;

  private bool ready;

  void Update() {
    if (!ready && manager.isControllerLinked(index)) {
      ready = true;
      readyEvent.Invoke();
    }
  }

}