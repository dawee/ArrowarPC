using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDirection : MonoBehaviour
{
    private enum Direction {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    [SerializeField]
    private Animator animator = default;

    [SerializeField]
    private TileSelector TileSelector = default;

    private Direction currentDirection = default;

    private void Awake() {
        var initialDirection = Random.Range(0, 4);
        SetDirection(initialDirection);
    }

    public void ChangeDirection(int playerIndex) {
        if (TileSelector.PlayerIndex != playerIndex) {
            return;
        }

        var newDirection = ((int) currentDirection + 1) % 4;
        SetDirection(newDirection);
    }

    private void SetDirection(int direction) {
        currentDirection = (Direction) direction;
        animator.SetInteger("direction", direction);
    }
}
