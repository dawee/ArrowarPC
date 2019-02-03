using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TestInputEnlighter : MonoBehaviour {

  [SerializeField]
  private RawImage background;

  [SerializeField]
  private InputEventStream input;

  private void LightOn() {
    background.color = Color.red;
  }

  private void LightOff() {
    background.color = Color.white;
  }

  void Start() {
    // input.FromAxis(InputEventStream.AxisType.Right).On.Subscribe(_ => LightOn());
    // input.FromAxis(InputEventStream.AxisType.Right).Off.Subscribe(_ => LightOff());
  }

}