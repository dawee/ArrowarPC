using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileDirection : MonoBehaviour
{
    public enum Direction {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    [SerializeField]
    private Animator animator = default;

    [SerializeField]
    private TileSelector TileSelector = default;

    [SerializeField]
    private PushedItemsEventNode pushedItems = new PushedItemsEventNode();

    [SerializeField]
    private RectTransform rectTransform;

    public PushedItemsEventNode PushedItems {
        get {
            return pushedItems;
        }
    }

    private Direction currentDirection = default;

    public HashSet<Item> Items {
        get;
        private set;
    } = new HashSet<Item>();

    public Direction CurrentDirection {
        get {
            return currentDirection;
        }
    }

    public void PushItems() {
        if (Items.Count > 0) {
            Debug.Log("PushItems()");

            switch(currentDirection) {
                case Direction.Left:
                    pushedItems.Left.Invoke(
                        new PushedItemsEventNode.Data(
                            this,
                            PushedItemsEventNode.Data.Direction.Left
                        )
                    );
                    break;
                case Direction.Right:
                    pushedItems.Right.Invoke(
                        new PushedItemsEventNode.Data(
                            this,
                            PushedItemsEventNode.Data.Direction.Right
                        )
                    );
                    break;
                case Direction.Down:
                    pushedItems.Down.Invoke(
                        new PushedItemsEventNode.Data(
                            this,
                            PushedItemsEventNode.Data.Direction.Down
                        )
                    );
                    break;
                case Direction.Up:
                    pushedItems.Up.Invoke(
                        new PushedItemsEventNode.Data(
                            this,
                            PushedItemsEventNode.Data.Direction.Up
                        )
                    );
                    break;
            };
        }
    }

    public void PullItems(PushedItemsEventNode.Data data) {
        foreach (var item in data.Origin.Items) {
            item.Move(data.Origin, this);
        }

        data.Origin.Items.Clear();
    }

    private void Awake() {
        var initialDirection = Random.Range(0, 4);
        SetDirection(initialDirection);
    }

    private void Update() {
        PushItems();
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

    public void InstantiateItem(Item.ItemType itemType) {
        var item = Item.Instantiate(itemType, transform.parent, rectTransform.anchoredPosition);
        var action = new UnityAction<ItemMoved>(GiveItem);

        item.Moved.AddListener(action);
        Items.Add(item);
    }

    public static void GiveItem(ItemMoved data) {
        data.Item.UpdatePosition(data.tileDirection.rectTransform.anchoredPosition);
        data.tileDirection.Items.Add(data.Item);
    }
}
