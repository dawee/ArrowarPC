using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

// public struct AxesMapping {
//   public string X {get; private set;}
// };

// public class ReactiveAxis {
//   private string name;

//   public ReactiveAxis(string name) {
//     this.name = name;  
//   }

  
// }


// public class Player {
//   public enum Name { Player1, Player2 };
//   public IReactiveProperty<bool> Ready  { get; private set; }
//   private AxesMapping? axes;
//   public AxesMapping Axes {
//     set  {
//       axes = value;
//       Ready.Value = true;
//     }
//   }

//   public ReactiveAxis X {
//     get {
//       return new ReactiveAxis(axes.Value.X);
//     }
//   }
// }


public class InputEventStream : MonoBehaviour {



  public class Axis {

    private string name;
    private int direction;

    public Axis(string name, int direction) {
      this.name = name;
      this.direction = direction;
    }

    public IObservable<long> On {
      get {
        return Observable.EveryUpdate()
          .Where(_ => Input.GetAxis(name) * direction > 0);
      }
    }

    public IObservable<long> Off {
      get {
        return Observable.EveryUpdate()
          .Where(_ => {
            return!(Input.GetAxis(name) * direction > 0);
          });
      }
    }

  }

  public enum AxisType { Left, Right };

  public Axis FromAxis(AxisType type) {
    switch (type) {
      case AxisType.Left:
        return new Axis("Horizontal_1", -1);
      case AxisType.Right:
        return new Axis("Horizontal_1", 1);
      default:
        return new Axis("", 0);
    }
  }

  void Update() {
    Debug.Log(string.Format("Fire2 : {0}", Input.GetAxis("Fire2")));

    for (int joystick = 1; joystick <= 3; joystick++) {
      var axisName = string.Format("ARW_X_Joystick{0}", joystick);
      Debug.Log(string.Format("Joystick {0}: {1}", joystick, Input.GetAxis(axisName)));
    }
  }

}