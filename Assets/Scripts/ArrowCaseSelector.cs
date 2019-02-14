using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ArrowCaseSelector : MonoBehaviour {

    [SerializeField]
    private Animator animator = default;

    [SerializeField]
    private ArrowCaseMoveSelection move = new ArrowCaseMoveSelection();

    public ArrowCaseMoveSelection Move {
        get {
            return move;
        }
    }

    private int playerIndex;

    public int PlayerIndex {
        get {
            return playerIndex;
        }

        set {
            SelectForPlayer(value);
        }
    }

    [SerializeField]
    private ArrowCaseSetup setup = default;

    private int lastMoveFrame = 0;

    private bool CanChangeSelection() {
        return lastMoveFrame != Time.frameCount;
    }

    public void SelectForPlayer(int playerIndex) {
        if (CanChangeSelection()) {
            this.playerIndex = playerIndex;
            animator.SetInteger("select", playerIndex);
            lastMoveFrame = Time.frameCount;
        }
    }

    public void Unselect() {
        SelectForPlayer(0);
    }

    public bool HasSelection() {
        return playerIndex > 0;
    }

    public void RequestMoveLeft(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Left.Invoke(this);            
        }
    }

    public void RequestMoveRight(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Right.Invoke(this);            
        }
    }

    public void RequestMoveDown(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Down.Invoke(this);            
        }
    }

    public void RequestMoveUp(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Up.Invoke(this);            
        }
    }

    public void StealSelectionFrom(ArrowCaseSelector origin) {
        if (CanChangeSelection() && origin.HasSelection() && !HasSelection()) {
            SelectForPlayer(origin.playerIndex);
            origin.Unselect();
        }
    }

}
