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
    private static readonly Vector2 gridSize = new Vector2(8, 5);

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

            UnityAction<ArrowCaseSelector> action = new UnityAction<ArrowCaseSelector>(nearSelector.StealSelectionFrom);
            UnityEventTools.AddPersistentListener(moveEvent, action);
        }
    }

    [MenuItem("GameObject/UI/Arrowar Grid")]
    static void CreateGrid() {
        var casesCount = Mathf.RoundToInt(gridSize.x * gridSize.y);
        var arrowCasePrefab = Resources.Load<GameObject>("ArrowCase");
        var arrowCaseRectTransform = arrowCasePrefab.GetComponent<RectTransform>();
        var grid = new GameObject("Arrowar Grid");
        var gridRectTransform = grid.AddComponent<RectTransform>();
        var setup = grid.AddComponent<ArrowCaseSetup>();
        var layout = grid.AddComponent<GridLayoutGroup>();
        var cases = new Dictionary<Vector2, GameObject>();
        var controllers = new Dictionary<int, Controller>();

        for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex) {
            controllers[playerIndex] = grid.AddComponent<Controller>();
            controllers[playerIndex].Index = playerIndex;
        }

        layout.cellSize = arrowCaseRectTransform.sizeDelta;
        layout.startCorner = GridLayoutGroup.Corner.LowerLeft;

        for (int index = 0; index < casesCount; ++index) {
            var position = GetGridPosition(index);
            var arrowCase = Object.Instantiate(arrowCasePrefab, Vector3.zero, Quaternion.identity);
            var selector = arrowCase.GetComponent<ArrowCaseSelector>();
            var serializedSelector = new UnityEditor.SerializedObject(selector);
            var setupProperty = serializedSelector.FindProperty("setup");
            
            setupProperty.objectReferenceValue = setup;

            serializedSelector.ApplyModifiedProperties();

            arrowCase.transform.SetParent(grid.transform);
            arrowCase.name = string.Format("ArrowCase {0} (Pos {1};{2})", index, position.x, position.y);
            cases[position] = arrowCase;

            for (var playerIndex = 1; playerIndex <= playersCount; ++playerIndex) {
                UnityEventTools.AddIntPersistentListener(
                    controllers[playerIndex].LeftInput.OnEvent,
                    new UnityAction<int>(selector.RequestMoveLeft),
                    playerIndex
                );

                UnityEventTools.AddIntPersistentListener(
                    controllers[playerIndex].RightInput.OnEvent,
                    new UnityAction<int>(selector.RequestMoveRight),
                    playerIndex
                );

                UnityEventTools.AddIntPersistentListener(
                    controllers[playerIndex].DownInput.OnEvent,
                    new UnityAction<int>(selector.RequestMoveDown),
                    playerIndex
                );

                UnityEventTools.AddIntPersistentListener(
                    controllers[playerIndex].UpInput.OnEvent,
                    new UnityAction<int>(selector.RequestMoveUp),
                    playerIndex
                );
            }

        }

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

        gridRectTransform.sizeDelta = new Vector2(
            gridSize.x * arrowCaseRectTransform.sizeDelta.x,
            gridSize.y * arrowCaseRectTransform.sizeDelta.y
        );
    }
}