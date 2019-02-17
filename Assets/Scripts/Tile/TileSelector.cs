using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileSelector : MonoBehaviour {

    [SerializeField]
    private Animator animator = default;

    [SerializeField]
    private TileMoveSelection move = new TileMoveSelection();

    [SerializeField]
    private TileSetup setup = default;

    private int playerIndex;
    private int lastMoveFrame = 0;

    public TileMoveSelection Move {
        get {
            return move;
        }
    }

    public int PlayerIndex {
        get {
            return playerIndex;
        }

        set {
            SelectForPlayer(value);
        }
    }

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
                new TileMoveSelection.Data(
                    this,
                    TileMoveSelection.Data.Direction.Left
                )
            );
        }
    }

    public void RequestMoveRight(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Right.Invoke(
                new TileMoveSelection.Data(
                    this,
                    TileMoveSelection.Data.Direction.Right
                )
            );
        }
    }

    public void RequestMoveDown(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Down.Invoke(
                new TileMoveSelection.Data(
                    this,
                    TileMoveSelection.Data.Direction.Down
                )
            );
        }
    }

    public void RequestMoveUp(int playerIndex) {
        if (CanChangeSelection() && this.playerIndex == playerIndex) {
            move.Up.Invoke(
                new TileMoveSelection.Data(
                    this,
                    TileMoveSelection.Data.Direction.Up
                )
            );
        }
    }

    public void StealSelectionFrom(TileMoveSelection.Data data) {
        if (CanChangeSelection() && data.Origin.HasSelection()) {
            if (HasSelection()) {
                switch(data.InitialDirection) {
                    case TileMoveSelection.Data.Direction.Left:
                        move.Left.Invoke(data);
                        break;
                    case TileMoveSelection.Data.Direction.Right:
                        move.Right.Invoke(data);
                        break;
                    case TileMoveSelection.Data.Direction.Down:
                        move.Down.Invoke(data);
                        break;
                    case TileMoveSelection.Data.Direction.Up:
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
