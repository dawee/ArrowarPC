using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArrowCaseSelector : MonoBehaviour {

    [SerializeField]
    private Animator animator;

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

    public void RequestMoveLeft(int playerIndex) {
        Debug.Log(string.Format("RequestMoveLeft {0}", playerIndex));
        if (this.playerIndex == playerIndex) {
            move.Left.Invoke(this);            
        }
    }

    public void RequestMoveRight(int playerIndex) {
        if (this.playerIndex == playerIndex) {
            move.Right.Invoke(this);            
        }
    }

    public void RequestMoveDown(int playerIndex) {
        if (this.playerIndex == playerIndex) {
            move.Down.Invoke(this);            
        }
    }

    public void RequestMoveUp(int playerIndex) {
        if (this.playerIndex == playerIndex) {
            move.Up.Invoke(this);            
        }
    }

    public void StealSelectionFrom(ArrowCaseSelector origin) {
        if (origin.HasSelection() && !HasSelection()) {
            SelectForPlayer(origin.playerIndex);
            origin.Unselect();
        }
    }

}
