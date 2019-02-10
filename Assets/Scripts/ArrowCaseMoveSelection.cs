using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ArrowCaseMoveSelection {
    
    [System.Serializable]
    public class EventType : UnityEvent<ArrowCaseSelector> {} 

    [SerializeField]
    private EventType right = new EventType();

    public EventType Right {
        get {
            return right;
        }
    }

    [SerializeField]
    private EventType up = new EventType();

    public EventType Up {
        get {
            return up;
        }
    }

    [SerializeField]
    private EventType down = new EventType();

    public EventType Down {
        get {
            return down;
        }
    }

    [SerializeField]
    private EventType left = new EventType();

    public EventType Left {
        get {
            return left;
        }
    }
}
