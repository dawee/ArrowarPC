﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
  public enum ItemType
  {
    Heart,
    Shield,
    Bomb
  };

  public interface Target
  {
    void OnMoveDone(Item item);
  }

  public class TileDirectionTarget : Target
  {
    private TileDirection tileDirection;

    public TileDirectionTarget(TileDirection tileDirection)
    {
      this.tileDirection = tileDirection;
    }

    public void OnMoveDone(Item item)
    {
      item.moved.Invoke(new ItemMoved(item, tileDirection));
    }
  }

  public class PlayerTarget : Target
  {
    private PlayerScript player;

    public PlayerTarget(PlayerScript player)
    {
      this.player = player;
    }

    public void OnMoveDone(Item item)
    {
      player.OnItemDropped(item);
    }
  }

  [SerializeField]
  private ItemType type = default;

  [SerializeField]
  private Image image = default;

  [SerializeField]
  private RectTransform rectTransform;

  [SerializeField]
  private Animator animator = default;

  [SerializeField]
  private Sprite heartSprite = default;

  [SerializeField]
  private Sprite shieldSprite = default;

  [SerializeField]
  private Sprite bombSprite = default;

  [SerializeField]
  private ItemMovedEvent moved = default;

  public ItemMovedEvent Moved
  {
    get
    {
      return moved;
    }
  }

  private Target target;
  private Vector2? nextPosition = null;

  private void OnValidate()
  {
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

  public void TriggerMove(TileDirection origin)
  {
    switch (origin.CurrentDirection)
    {
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

  public void Move(TileDirection origin, TileDirection tileDirectionTarget)
  {
    target = new TileDirectionTarget(tileDirectionTarget);
    TriggerMove(origin);
  }

  public void Move(TileDirection origin, PlayerScript playerTarget)
  {
    target = new PlayerTarget(playerTarget);
    TriggerMove(origin);
  }

  public void OnMoveDone()
  {
    if (target == null)
    {
      return;
    }

    target.OnMoveDone(this);
  }

  public void UpdatePosition(Vector2 position)
  {
    var isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");

    if (isIdle)
    {
      rectTransform.anchoredPosition = position;
    }
    else
    {
      nextPosition = position;
    }
  }

  public static Item Instantiate(ItemType itemType, Transform parent, Vector2 position)
  {
    var prefab = Resources.Load<GameObject>("Item");
    var gameObject = Object.Instantiate(prefab) as GameObject;
    var item = gameObject.GetComponent<Item>();

    item.type = itemType;
    gameObject.transform.SetParent(parent);
    item.rectTransform.anchorMin = Vector2.zero;
    item.rectTransform.anchorMax = Vector2.zero;
    item.rectTransform.localScale = Vector2.one;
    item.OnValidate();
    item.UpdatePosition(position);

    return item;
  }

  private void Update()
  {
    if (nextPosition.HasValue)
    {
      rectTransform.anchoredPosition = nextPosition.Value;
      nextPosition = null;

      animator.SetTrigger("Reset");
    }
  }
}
