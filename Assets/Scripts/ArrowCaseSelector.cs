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
            move.Left.Invoke(
                new ArrowCaseMoveSelection.Data(
                    this,
                    ArrowCaseMoveSelection.Data.Direction.Left
                )
            );
        }
    }

    public void RequestMoveRight(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Right.Invoke(
                new ArrowCaseMoveSelection.Data(
                    this,
                    ArrowCaseMoveSelection.Data.Direction.Right
                )
            );
        }
    }

    public void RequestMoveDown(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Down.Invoke(
                new ArrowCaseMoveSelection.Data(
                    this,
                    ArrowCaseMoveSelection.Data.Direction.Down
                )
            );
        }
    }

    public void RequestMoveUp(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Up.Invoke(
                new ArrowCaseMoveSelection.Data(
                    this,
                    ArrowCaseMoveSelection.Data.Direction.Up
                )
            );
        }
    }

    public void StealSelectionFrom(ArrowCaseMoveSelection.Data data) {
        if (CanChangeSelection() && data.Origin.HasSelection()) {
            if (HasSelection()) {
                switch(data.InitialDirection) {
                    case ArrowCaseMoveSelection.Data.Direction.Left:
                        move.Left.Invoke(data);
                        break;
                    case ArrowCaseMoveSelection.Data.Direction.Right:
                        move.Right.Invoke(data);
                        break;
                    case ArrowCaseMoveSelection.Data.Direction.Down:
                        move.Down.Invoke(data);
                        break;
                    case ArrowCaseMoveSelection.Data.Direction.Up:
                        move.Up.Invoke(data);
                        break;
                };
            } else {
                SelectForPlayer(data.Origin.playerIndex);
                data.Origin.Unselect();
            }
        }
    }

}
