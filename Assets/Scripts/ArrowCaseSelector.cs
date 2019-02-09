using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArrowCaseSelector : MonoBehaviour {

    [System.Serializable]
    public class MoveRequestEvent : UnityEvent<ArrowCaseSelector> {} 

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private MoveRequestEvent moveLeftRequestEvent;

    [SerializeField]
    private MoveRequestEvent moveRightRequestEvent;

    [SerializeField]
    private MoveRequestEvent moveUpRequestEvent;

    [SerializeField]
    private MoveRequestEvent moveDownRequestEvent;

    private int playerIndex;

    public int PlayerIndex {
        get {
            return playerIndex;
        }

        set {
            SelectForPlayer(value);
        }
    }

    public void SelectForPlayer(int playerIndex) {
        this.playerIndex = playerIndex;
        animator.SetInteger("select", playerIndex);
    }

    public void Unselect() {
        SelectForPlayer(0);
    }

    public bool HasSelection() {
        return playerIndex > 0;
    }

    public void RequestMoveLeft() {
        moveLeftRequestEvent.Invoke(this);
    }

    public void RequestMoveRight() {
        moveRightRequestEvent.Invoke(this);
    }

    public void RequestMoveDown() {
        moveDownRequestEvent.Invoke(this);
    }

    public void RequestMoveUp() {
        moveUpRequestEvent.Invoke(this);
    }

    public void StealSelectionFrom(ArrowCaseSelector origin) {
        if (origin.HasSelection() && !HasSelection()) {
            SelectForPlayer(origin.playerIndex);
            origin.Unselect();
        }
    }

}
