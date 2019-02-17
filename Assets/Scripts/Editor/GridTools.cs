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
    private static readonly Vector2 playerBaseSize = new Vector2(3, 2);
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

    static void ConnectNearTile(
        Dictionary<Vector2, GameObject> tiles,
        Vector2 nearPosition,
        TileSelector selector,
        TileMoveSelection.EventType moveEvent
    ) {
        if (tiles.ContainsKey(nearPosition)) {
            var nearCase = tiles[nearPosition];
            var nearSelector = nearCase.GetComponent<TileSelector>();
            var action = new UnityAction<TileMoveSelection.Data>(nearSelector.StealSelectionFrom);

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
        var TilePrefab = Resources.Load<GameObject>("Tile");
        var TileRectTransform = TilePrefab.GetComponent<RectTransform>();
        var setup = grid.GetComponent<TileSetup>();
        var gridRectTransform = grid.GetComponent<RectTransform>();

        for (int index = 0; index < tilesCount; ++index) {
            var position = GetGridPosition(index);

            if (!deadCells.Contains(position)) {
                var Tile = Object.Instantiate(TilePrefab, Vector3.zero, Quaternion.identity);
                var selector = Tile.GetComponent<TileSelector>();
                var direction = Tile.GetComponent<TileDirection>();
                var rectTransform = Tile.GetComponent<RectTransform>();
                var serializedSelector = new UnityEditor.SerializedObject(selector);
                var setupProperty = serializedSelector.FindProperty("setup");

                setupProperty.objectReferenceValue = setup;

                serializedSelector.ApplyModifiedProperties();

                Tile.transform.SetParent(grid.transform);

                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.anchoredPosition =
                    (position * TileRectTransform.sizeDelta.x) +
                    new Vector2(TileRectTransform.sizeDelta.x / 2, TileRectTransform.sizeDelta.x / 2);

                Tile.name = string.Format("Tile Pos {1};{2}", index, position.x, position.y);
                tiles[position] = Tile;

                AddSelectorMoveRequestListeners(selector, controllers);
                AddSelectorDirectionChangeListeners(direction, controllers);
            }
        }

        gridRectTransform.sizeDelta = new Vector2(
            gridSize.x * TileRectTransform.sizeDelta.x,
            gridSize.y * TileRectTransform.sizeDelta.y
        );

        return tiles;
    }

    [MenuItem("GameObject/UI/Arrowar Grid")]
    static void CreateGrid() {
        var tilesCount = Mathf.RoundToInt(gridSize.x * gridSize.y);
        var grid = new GameObject("Arrowar Grid");
        var gridRectTransform = grid.AddComponent<RectTransform>();
        var setup = grid.AddComponent<TileSetup>();
        var controllers = new Dictionary<int, Controller>();

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

        foreach (var TileItem in tiles) {
            var Tile = TileItem.Value;
            var selector = Tile.GetComponent<TileSelector>();

            ConnectNearTile(
                tiles,
                TileItem.Key + Vector2.left,
                selector,
                selector.Move.Left
            );

            ConnectNearTile(
                tiles,
                TileItem.Key + Vector2.right,
                selector,
                selector.Move.Right
            );

            ConnectNearTile(
                tiles,
                TileItem.Key + Vector2.down,
                selector,
                selector.Move.Down
            );

            ConnectNearTile(
                tiles,
                TileItem.Key + Vector2.up,
                selector,
                selector.Move.Up
            );
        }

        if (Selection.activeGameObject) {
            grid.transform.SetParent(Selection.activeGameObject.transform);
        }

        gridRectTransform.anchoredPosition = Vector2.zero;
        gridRectTransform.localPosition = Vector2.zero;
        gridRectTransform.localScale = Vector2.one;
    }
}