using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemType {
        Heart,
        Shield,
        Bomb
    };

    [SerializeField]
    private ItemType type = default;

    [SerializeField]
    private Image image = default;

    [SerializeField]
    private Animator animator = default;

    [SerializeField]
    private Sprite heartSprite = default;

    [SerializeField]
    private Sprite shieldSprite = default;
    
    [SerializeField]
    private Sprite bombSprite = default;

    [SerializeField]
    private TileDirectionEvent onMoveDone = default;

    public ItemType Type {
        get {
            return type;
        }
        set {
            type = value;
        }
    }

    private TileDirection targetSquare;

    private void OnValidate() {
        switch (type)
        {
            case ItemType.Heart:
                image.sprite = heartSprite;
                break;
            case ItemType.Shield:
                image.sprite = shieldSprite;
                break;
            case ItemType.Bomb:
                image.sprite = bombSprite;
                break;
        }
    }

    public void Move(TileDirection origin, TileDirection target) {
        targetSquare = target;

        if (target == null) {
            return;
        }

        switch (origin.CurrentDirection) {
            case TileDirection.Direction.Up:
                animator.SetTrigger("MoveUp");
                break;
            case TileDirection.Direction.Right:
                animator.SetTrigger("MoveRight");
                break;
            case TileDirection.Direction.Down:
                animator.SetTrigger("MoveDown");
                break;
            case TileDirection.Direction.Left:
                animator.SetTrigger("MoveLeft");
                break;
        }
    }

    public void OnMoveDone() {
        if (targetSquare == null) {
            return;
        }

        onMoveDone.Invoke(targetSquare);
    }
}
