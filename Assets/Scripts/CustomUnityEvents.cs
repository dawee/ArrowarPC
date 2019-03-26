using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemMoved
{
  public Item Item { get; private set; }
  public TileDirection tileDirection { get; private set; }

  public ItemMoved(Item item, TileDirection tileDirection)
  {
    this.Item = item;
    this.tileDirection = tileDirection;
  }
}

[System.Serializable]
public class ItemMovedEvent : UnityEvent<ItemMoved> { };

[System.Serializable]
public class ItemTypeEvent : UnityEvent<Item.ItemType> { };

[System.Serializable]
public class TileSelectionMoveEventNode
{

  [System.Serializable]
  public class Data
  {
    public enum Direction
    {
      Left,
      Right,
      Down,
      Up
    }

    public delegate void Listener(Data data);

    public TileSelector Origin { get; private set; }
    public Direction InitialDirection { get; private set; }

    public Data(TileSelector origin, Direction initialDirection)
    {
      Origin = origin;
      InitialDirection = initialDirection;
    }
  }

  [System.Serializable]
  public class EventType : UnityEvent<Data> { }

  [SerializeField]
  private EventType right = new EventType();

  public EventType Right
  {
    get
    {
      return right;
    }
  }

  [SerializeField]
  private EventType up = new EventType();

  public EventType Up
  {
    get
    {
      return up;
    }
  }

  [SerializeField]
  private EventType down = new EventType();

  public EventType Down
  {
    get
    {
      return down;
    }
  }

  [SerializeField]
  private EventType left = new EventType();

  public EventType Left
  {
    get
    {
      return left;
    }
  }
}

[System.Serializable]
public class PushedItemsEventNode
{

  [System.Serializable]
  public class Data
  {
    public enum Direction
    {
      Left,
      Right,
      Down,
      Up
    }

    public delegate void Listener(Data data);

    public TileDirection Origin { get; private set; }
    public Direction InitialDirection { get; private set; }

    public Data(TileDirection origin, Direction initialDirection)
    {
      Origin = origin;
      InitialDirection = initialDirection;
    }
  }

  [System.Serializable]
  public class EventType : UnityEvent<Data> { }

  [SerializeField]
  private EventType right = new EventType();

  public EventType Right
  {
    get
    {
      return right;
    }
  }

  [SerializeField]
  private EventType up = new EventType();

  public EventType Up
  {
    get
    {
      return up;
    }
  }

  [SerializeField]
  private EventType down = new EventType();

  public EventType Down
  {
    get
    {
      return down;
    }
  }

  [SerializeField]
  private EventType left = new EventType();

  public EventType Left
  {
    get
    {
      return left;
    }
  }
}
