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

    static void ConnectNearCase(
        Dictionary<Vector2, GameObject> cases,
        Vector2 nearPosition,
        ArrowCaseSelector selector,
        ArrowCaseMoveSelection.EventType moveEvent
    ) {
        if (cases.ContainsKey(nearPosition)) {
            var nearCase = cases[nearPosition];
            var nearSelector = nearCase.GetComponent<ArrowCaseSelector>();
            var action = new UnityAction<ArrowCaseMoveSelection.Data>(nearSelector.StealSelectionFrom);

            UnityEventTools.AddPersistentListener(moveEvent, action);
        }
    }


    static void  AddSelectorDirectionChangeListeners(ArrowCaseDirection direction, Dictionary<int, Controller> controllers) {
        for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex) {
            UnityEventTools.AddIntPersistentListener(
                controllers[playerIndex].DownButton.TurnOn,
                new UnityAction<int>(direction.ChangeDirection),
                playerIndex
            );
        }
    }

    static void AddSelectorMoveRequestListeners(ArrowCaseSelector selector, Dictionary<int, Controller> controllers) {
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

    static Dictionary<Vector2, GameObject> CreateArrowCases(int casesCount, GameObject grid, Dictionary<int, Controller> controllers) {
        var cases = new Dictionary<Vector2, GameObject>();
        var arrowCasePrefab = Resources.Load<GameObject>("ArrowCase");
        var arrowCaseRectTransform = arrowCasePrefab.GetComponent<RectTransform>();
        var setup = grid.GetComponent<ArrowCaseSetup>();
        var gridRectTransform = grid.GetComponent<RectTransform>();

        for (int index = 0; index < casesCount; ++index) {
            var position = GetGridPosition(index);

            if (!deadCells.Contains(position)) {
                var arrowCase = Object.Instantiate(arrowCasePrefab, Vector3.zero, Quaternion.identity);
                var selector = arrowCase.GetComponent<ArrowCaseSelector>();
                var direction = arrowCase.GetComponent<ArrowCaseDirection>();
                var rectTransform = arrowCase.GetComponent<RectTransform>();
                var serializedSelector = new UnityEditor.SerializedObject(selector);
                var setupProperty = serializedSelector.FindProperty("setup");

                setupProperty.objectReferenceValue = setup;

                serializedSelector.ApplyModifiedProperties();

                arrowCase.transform.SetParent(grid.transform);

                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.anchoredPosition =
                    (position * arrowCaseRectTransform.sizeDelta.x) +
                    new Vector2(arrowCaseRectTransform.sizeDelta.x / 2, arrowCaseRectTransform.sizeDelta.x / 2);

                arrowCase.name = string.Format("ArrowCase Pos {1};{2}", index, position.x, position.y);
                cases[position] = arrowCase;

                AddSelectorMoveRequestListeners(selector, controllers);
                AddSelectorDirectionChangeListeners(direction, controllers);
            }
        }

        gridRectTransform.sizeDelta = new Vector2(
            gridSize.x * arrowCaseRectTransform.sizeDelta.x,
            gridSize.y * arrowCaseRectTransform.sizeDelta.y
        );

        return cases;
    }

    [MenuItem("GameObject/UI/Arrowar Grid")]
    static void CreateGrid() {
        var casesCount = Mathf.RoundToInt(gridSize.x * gridSize.y);
        var grid = new GameObject("Arrowar Grid");
        var gridRectTransform = grid.AddComponent<RectTransform>();
        var setup = grid.AddComponent<ArrowCaseSetup>();
        var controllers = new Dictionary<int, Controller>();

        for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex) {
            controllers[playerIndex] = grid.AddComponent<Controller>();
            controllers[playerIndex].Index = playerIndex;
        }

        var cases = CreateArrowCases(casesCount, grid, controllers);

        UnityEventTools.AddIntPersistentListener(
            controllers[1].ReadyEvent,
            new UnityAction<int>(cases.First().Value.GetComponent<ArrowCaseSelector>().SelectForPlayer),
            1
        );

        UnityEventTools.AddIntPersistentListener(
            controllers[2].ReadyEvent,
            new UnityAction<int>(cases.Last().Value.GetComponent<ArrowCaseSelector>().SelectForPlayer),
            2
        );

        foreach (var arrowCaseItem in cases) {
            var arrowCase = arrowCaseItem.Value;
            var selector = arrowCase.GetComponent<ArrowCaseSelector>();

            ConnectNearCase(
                cases,
                arrowCaseItem.Key + Vector2.left,
                selector,
                selector.Move.Left
            );

            ConnectNearCase(
                cases,
                arrowCaseItem.Key + Vector2.right,
                selector,
                selector.Move.Right
            );

            ConnectNearCase(
                cases,
                arrowCaseItem.Key + Vector2.down,
                selector,
                selector.Move.Down
            );

            ConnectNearCase(
                cases,
                arrowCaseItem.Key + Vector2.up,
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