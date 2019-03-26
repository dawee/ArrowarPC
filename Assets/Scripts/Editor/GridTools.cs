using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerBase
{
  private PlayerScript player;
  private Rect rect;

  public PlayerScript Player
  {
    get
    {
      return player;
    }
  }

  public PlayerBase(PlayerScript player, Rect rect)
  {
    this.rect = rect;
    this.player = player;
  }

  public bool Contains(Vector2 position)
  {
    return rect.Contains(position);
  }
}

[CanEditMultipleObjects]
public class GridTools
{
  private static readonly int playersCount = 2;
  private static readonly Vector2 playerBaseSize = new Vector2(2, 2);
  private static readonly Vector2 gridSize = new Vector2(14, 6);
  private static readonly Vector2[] deadCells = InitDeadCells();


  static Vector2[] InitDeadCells()
  {
    return Enumerable
        .Range(0, Mathf.RoundToInt(playerBaseSize.x))
        .Select(x =>
            Enumerable
                .Range(0, Mathf.RoundToInt(playerBaseSize.y))
                .Select(y => new Vector2[] {
                        new Vector2(x, y),
                        new Vector2(gridSize.x - x - 1, y)
                })
                .SelectMany(a => a)
                .Distinct()
        )
        .SelectMany(a => a)
        .Distinct()
        .ToArray();
  }

  static Vector2 GetGridPosition(int index)
  {
    var yIndex = Mathf.FloorToInt(index / gridSize.x);
    var xIndex = Mathf.RoundToInt(index - yIndex * gridSize.x);

    return new Vector2(xIndex, yIndex);
  }

  static void ConnectNearTileMovedSelection(
      Dictionary<Vector2, GameObject> tiles,
      Vector2 nearPosition,
      TileSelector selector,
      TileSelectionMoveEventNode.EventType moveEvent
  )
  {
    if (tiles.ContainsKey(nearPosition))
    {
      var nearTile = tiles[nearPosition];
      var nearSelector = nearTile.GetComponent<TileSelector>();
      var action = new UnityAction<TileSelectionMoveEventNode.Data>(nearSelector.StealSelectionFrom);

      UnityEventTools.AddPersistentListener(moveEvent, action);
    }
  }

  static void ConnectNearTilePushedItems(
      Dictionary<Vector2, GameObject> tiles,
      Vector2 nearPosition,
      TileDirection direction,
      PushedItemsEventNode.EventType moveEvent,
      List<PlayerBase> playerBases
  )
  {
    var nearPlayerBase = playerBases.FirstOrDefault(playerBase => playerBase.Contains(nearPosition));

    if (nearPlayerBase != null)
    {
      var action = new UnityAction<PushedItemsEventNode.Data>(nearPlayerBase.Player.PullItems);

      UnityEventTools.AddPersistentListener(moveEvent, action);
    }
    else if (tiles.ContainsKey(nearPosition))
    {
      var nearTile = tiles[nearPosition];
      var nearDirection = nearTile.GetComponent<TileDirection>();
      var action = new UnityAction<PushedItemsEventNode.Data>(nearDirection.PullItems);

      UnityEventTools.AddPersistentListener(moveEvent, action);
    }
  }

  static void AddSelectorDirectionChangeListeners(TileDirection direction, Dictionary<int, Controller> controllers)
  {
    for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex)
    {
      UnityEventTools.AddIntPersistentListener(
          controllers[playerIndex].DownButton.TurnOn,
          new UnityAction<int>(direction.ChangeDirection),
          playerIndex
      );
    }
  }

  static void AddSelectorMoveRequestListeners(TileSelector selector, Dictionary<int, Controller> controllers)
  {
    for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex)
    {
      UnityEventTools.AddIntPersistentListener(
          controllers[playerIndex].DPadLeft.TurnOn,
          new UnityAction<int>(selector.RequestMoveLeft),
          playerIndex
      );

      UnityEventTools.AddIntPersistentListener(
          controllers[playerIndex].DPadRight.TurnOn,
          new UnityAction<int>(selector.RequestMoveRight),
          playerIndex
      );

      UnityEventTools.AddIntPersistentListener(
          controllers[playerIndex].DPadDown.TurnOn,
          new UnityAction<int>(selector.RequestMoveDown),
          playerIndex
      );

      UnityEventTools.AddIntPersistentListener(
          controllers[playerIndex].DPadUp.TurnOn,
          new UnityAction<int>(selector.RequestMoveUp),
          playerIndex
      );
    }
  }

  static void InitTileRectTransform(GameObject tile, RectTransform referenceRectTransform, Vector2 position)
  {
    var rectTransform = tile.GetComponent<RectTransform>();

    rectTransform.anchorMin = Vector2.zero;
    rectTransform.anchorMax = Vector2.zero;
    rectTransform.anchoredPosition =
        (position * referenceRectTransform.sizeDelta.x) +
        new Vector2(referenceRectTransform.sizeDelta.x / 2, referenceRectTransform.sizeDelta.x / 2);
  }

  static void InitGridLayerRectTransform(GameObject layer)
  {
    var rectTransform = layer.AddComponent<RectTransform>();
    rectTransform.anchorMin = Vector2.zero;
    rectTransform.anchorMax = Vector2.one;
    rectTransform.anchoredPosition = Vector2.zero;
    rectTransform.localPosition = Vector2.zero;
    rectTransform.localScale = Vector2.one;
    rectTransform.sizeDelta = Vector2.zero;
  }

  static Dictionary<Vector2, GameObject> CreateTiles(int tilesCount, GameObject grid, Dictionary<int, Controller> controllers)
  {
    var tiles = new Dictionary<Vector2, GameObject>();
    var tileBackgroundPrefab = Resources.Load<GameObject>("TileBackground");
    var tilePathPrefab = Resources.Load<GameObject>("TilePath");
    var tilePrefab = Resources.Load<GameObject>("TileArrow");
    var tileRectTransform = tilePrefab.GetComponent<RectTransform>();
    var setup = grid.GetComponent<TileSetup>();
    var gridRectTransform = grid.GetComponent<RectTransform>();

    // create layers
    var backgroundLayer = new GameObject("BackgroundLayer");
    var pathLayer = new GameObject("PathLayer");
    var arrowLayer = new GameObject("ArrowLayer");
    backgroundLayer.transform.SetParent(grid.transform);
    pathLayer.transform.SetParent(grid.transform);
    arrowLayer.transform.SetParent(grid.transform);

    for (int index = 0; index < tilesCount; ++index)
    {
      var position = GetGridPosition(index);

      if (!deadCells.Contains(position))
      {
        var tileBackground = PrefabUtility.InstantiatePrefab(tileBackgroundPrefab) as GameObject;
        var tilePath = PrefabUtility.InstantiatePrefab(tilePathPrefab) as GameObject;
        var tileArrow = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
        var selector = tileArrow.GetComponent<TileSelector>();
        var direction = tileArrow.GetComponent<TileDirection>();
        var serializedSelector = new UnityEditor.SerializedObject(selector);
        var setupProperty = serializedSelector.FindProperty("setup");

        setupProperty.objectReferenceValue = setup;

        serializedSelector.ApplyModifiedProperties();

        // Add tile element to the right layer
        tileBackground.transform.SetParent(backgroundLayer.transform);
        tilePath.transform.SetParent(pathLayer.transform);
        tileArrow.transform.SetParent(arrowLayer.transform);

        // Set tile element position
        InitTileRectTransform(tileBackground, tileRectTransform, position);
        InitTileRectTransform(tilePath, tileRectTransform, position);
        InitTileRectTransform(tileArrow, tileRectTransform, position);

        // Set tile element name
        tileBackground.name = string.Format("Background {0};{1}", position.x, position.y);
        tilePath.name = string.Format("Path {0};{1}", position.x, position.y);
        tileArrow.name = string.Format("Tile {0};{1}", position.x, position.y);

        tiles[position] = tileArrow;

        AddSelectorMoveRequestListeners(selector, controllers);
        AddSelectorDirectionChangeListeners(direction, controllers);
      }
    }

    gridRectTransform.sizeDelta = new Vector2(
        gridSize.x * tileRectTransform.sizeDelta.x,
        gridSize.y * tileRectTransform.sizeDelta.y
    );

    InitGridLayerRectTransform(backgroundLayer);
    InitGridLayerRectTransform(pathLayer);
    InitGridLayerRectTransform(arrowLayer);

    return tiles;
  }

  static PlayerBase CreatePlayerBase(GameObject prefab, GameObject parent, Vector2 position)
  {
    var playerArea = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
    var player = playerArea.GetComponentsInChildren<PlayerScript>()[0];

    playerArea.transform.SetParent(parent.transform);
    playerArea.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

    return new PlayerBase(player, new Rect(position, playerBaseSize));
  }

  [MenuItem("GameObject/UI/Arrowar Grid")]
  static void CreateGrid()
  {
    var tilesCount = Mathf.RoundToInt(gridSize.x * gridSize.y);
    var grid = new GameObject("Arrowar Grid");
    var gridRectTransform = grid.AddComponent<RectTransform>();
    var setup = grid.AddComponent<TileSetup>();
    var distributor = grid.AddComponent<ItemDistributor>();
    var controllers = new Dictionary<int, Controller>();
    var playerBases = new List<PlayerBase> {
      CreatePlayerBase(
        Resources.Load<GameObject>("Player1"),
        grid,
        Vector2.zero
      ),
      CreatePlayerBase(
        Resources.Load<GameObject>("Player2"),
        grid,
        Vector2.right * (gridSize.x - playerBaseSize.x)
      )
    };

    for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex)
    {
      controllers[playerIndex] = grid.AddComponent<Controller>();
      controllers[playerIndex].Index = playerIndex;
    }

    var tiles = CreateTiles(tilesCount, grid, controllers);

    UnityEventTools.AddIntPersistentListener(
        controllers[1].ReadyEvent,
        new UnityAction<int>(tiles.First().Value.GetComponent<TileSelector>().SelectForPlayer),
        1
    );

    UnityEventTools.AddIntPersistentListener(
        controllers[2].ReadyEvent,
        new UnityAction<int>(tiles.Last().Value.GetComponent<TileSelector>().SelectForPlayer),
        2
    );

    foreach (var tileItem in tiles)
    {
      var tile = tileItem.Value;
      var direction = tile.GetComponent<TileDirection>();
      var selector = tile.GetComponent<TileSelector>();

      ConnectNearTileMovedSelection(
          tiles,
          tileItem.Key + Vector2.left,
          selector,
          selector.Move.Left
      );

      ConnectNearTileMovedSelection(
          tiles,
          tileItem.Key + Vector2.right,
          selector,
          selector.Move.Right
      );

      ConnectNearTileMovedSelection(
          tiles,
          tileItem.Key + Vector2.down,
          selector,
          selector.Move.Down
      );

      ConnectNearTileMovedSelection(
          tiles,
          tileItem.Key + Vector2.up,
          selector,
          selector.Move.Up
      );

      ConnectNearTilePushedItems(
          tiles,
          tileItem.Key + Vector2.left,
          direction,
          direction.PushedItems.Left,
          playerBases
      );

      ConnectNearTilePushedItems(
          tiles,
          tileItem.Key + Vector2.right,
          direction,
          direction.PushedItems.Right,
          playerBases
      );

      ConnectNearTilePushedItems(
          tiles,
          tileItem.Key + Vector2.down,
          direction,
          direction.PushedItems.Down,
          playerBases
      );

      ConnectNearTilePushedItems(
          tiles,
          tileItem.Key + Vector2.up,
          direction,
          direction.PushedItems.Up,
          playerBases
      );

    }

    if (Selection.activeGameObject)
    {
      grid.transform.SetParent(Selection.activeGameObject.transform);
    }

    UnityEventTools.AddPersistentListener(
        distributor.Distributed,
        new UnityAction<Item.ItemType>(tiles.Last().Value.GetComponent<TileDirection>().InstantiateItem)
    );

    gridRectTransform.anchoredPosition = Vector2.zero;
    gridRectTransform.localPosition = Vector2.zero;
    gridRectTransform.localScale = Vector2.one;
  }
}