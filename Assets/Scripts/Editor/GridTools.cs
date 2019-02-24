using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CanEditMultipleObjects]
public class GridTools {

    private static readonly int playersCount = 2;
    private static readonly Vector2 playerBaseSize = new Vector2(2, 2);
    private static readonly Vector2 gridSize = new Vector2(14, 6);
    private static readonly Vector2[] deadCells = InitDeadCells();

    static Vector2[] InitDeadCells() {
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

    static Vector2 GetGridPosition(int index) {
        var yIndex = Mathf.FloorToInt(index / gridSize.x);
        var xIndex = Mathf.RoundToInt(index - yIndex * gridSize.x); 

        return new Vector2(xIndex, yIndex);
    }

    static void ConnectNearTileMovedSelection(
        Dictionary<Vector2, GameObject> tiles,
        Vector2 nearPosition,
        TileSelector selector,
        TileSelectionMoveEventNode.EventType moveEvent
    ) {
        if (tiles.ContainsKey(nearPosition)) {
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
        PushedItemsEventNode.EventType moveEvent
    ) {
        if (tiles.ContainsKey(nearPosition)) {
            var nearTile = tiles[nearPosition];
            var nearDirection = nearTile.GetComponent<TileDirection>();
            var action = new UnityAction<PushedItemsEventNode.Data>(nearDirection.PullItems);

            UnityEventTools.AddPersistentListener(moveEvent, action);
        }
    }

    static void  AddSelectorDirectionChangeListeners(TileDirection direction, Dictionary<int, Controller> controllers) {
        for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex) {
            UnityEventTools.AddIntPersistentListener(
                controllers[playerIndex].DownButton.TurnOn,
                new UnityAction<int>(direction.ChangeDirection),
                playerIndex
            );
        }
    }

    static void AddSelectorMoveRequestListeners(TileSelector selector, Dictionary<int, Controller> controllers) {
        for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex) {
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

    static Dictionary<Vector2, GameObject> CreateTiles(int tilesCount, GameObject grid, Dictionary<int, Controller> controllers) {
        var tiles = new Dictionary<Vector2, GameObject>();
        var tilePrefab = Resources.Load<GameObject>("Tile");
        var tileRectTransform = tilePrefab.GetComponent<RectTransform>();
        var setup = grid.GetComponent<TileSetup>();
        var gridRectTransform = grid.GetComponent<RectTransform>();

        for (int index = 0; index < tilesCount; ++index) {
            var position = GetGridPosition(index);

            if (!deadCells.Contains(position)) {
                var tile = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                var selector = tile.GetComponent<TileSelector>();
                var direction = tile.GetComponent<TileDirection>();
                var rectTransform = tile.GetComponent<RectTransform>();
                var serializedSelector = new UnityEditor.SerializedObject(selector);
                var setupProperty = serializedSelector.FindProperty("setup");

                setupProperty.objectReferenceValue = setup;

                serializedSelector.ApplyModifiedProperties();

                tile.transform.SetParent(grid.transform);

                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.anchoredPosition =
                    (position * tileRectTransform.sizeDelta.x) +
                    new Vector2(tileRectTransform.sizeDelta.x / 2, tileRectTransform.sizeDelta.x / 2);

                tile.name = string.Format("Tile Pos {1};{2}", index, position.x, position.y);
                tiles[position] = tile;

                AddSelectorMoveRequestListeners(selector, controllers);
                AddSelectorDirectionChangeListeners(direction, controllers);
            }
        }

        gridRectTransform.sizeDelta = new Vector2(
            gridSize.x * tileRectTransform.sizeDelta.x,
            gridSize.y * tileRectTransform.sizeDelta.y
        );

        return tiles;
    }

    [MenuItem("GameObject/UI/Arrowar Grid")]
    static void CreateGrid() {
        var tilesCount = Mathf.RoundToInt(gridSize.x * gridSize.y);
        var grid = new GameObject("Arrowar Grid");
        var gridRectTransform = grid.AddComponent<RectTransform>();
        var setup = grid.AddComponent<TileSetup>();
        var distributor = grid.AddComponent<ItemDistributor>();
        var controllers = new Dictionary<int, Controller>();
        var player1AreaPrefab = Resources.Load<GameObject>("Player1");
        var player2AreaPrefab = Resources.Load<GameObject>("Player2");

        for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex) {
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

        foreach (var tileItem in tiles) {
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
                direction.PushedItems.Left
            );

            ConnectNearTilePushedItems(
                tiles,
                tileItem.Key + Vector2.right,
                direction,
                direction.PushedItems.Right
            );

            ConnectNearTilePushedItems(
                tiles,
                tileItem.Key + Vector2.down,
                direction,
                direction.PushedItems.Down
            );

            ConnectNearTilePushedItems(
                tiles,
                tileItem.Key + Vector2.up,
                direction,
                direction.PushedItems.Up
            );

        }

        if (Selection.activeGameObject) {
            grid.transform.SetParent(Selection.activeGameObject.transform);
        }

        var player1Area = PrefabUtility.InstantiatePrefab(player1AreaPrefab) as GameObject;
        var player2Area = PrefabUtility.InstantiatePrefab(player2AreaPrefab) as GameObject;

        player1Area.transform.SetParent(grid.transform);
        player1Area.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        player2Area.transform.SetParent(grid.transform);
        player2Area.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        UnityEventTools.AddPersistentListener(
            distributor.Distributed,
            new UnityAction<Item.ItemType>(tiles.Last().Value.GetComponent<TileDirection>().InstantiateItem)
        );

        gridRectTransform.anchoredPosition = Vector2.zero;
        gridRectTransform.localPosition = Vector2.zero;
        gridRectTransform.localScale = Vector2.one;
    }
}