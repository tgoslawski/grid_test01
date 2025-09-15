using System;
using UnityEngine;
using R3;

public struct PointerEvent {
    public readonly Vector3 WorldPosition;
    public readonly Vector2Int Cell;
    public readonly int PointerId;
    public PointerEvent(Vector3 world, Vector2Int cell, int pointerId = 0) {
        WorldPosition = world; Cell = cell; PointerId = pointerId;
    }
}

[DefaultExecutionOrder(-100)]
public class InputService : MonoBehaviour {
    public readonly Subject<PointerEvent> PointerDown = new Subject<PointerEvent>();
    public readonly Subject<PointerEvent> PointerMove = new Subject<PointerEvent>();
    public readonly Subject<PointerEvent> PointerUp   = new Subject<PointerEvent>();
    [SerializeField] Camera mainCamera;
    [SerializeField] GridTilemapAdapter gridAdapter;
    void Awake() { if (mainCamera == null) mainCamera = Camera.main; }
    void Update() {
        if (Input.GetMouseButtonDown(0)) Emit(PointerDown);
        if (Input.GetMouseButton(0)) Emit(PointerMove);
        if (Input.GetMouseButtonUp(0)) Emit(PointerUp);
    }
    void Emit(Subject<PointerEvent> s) {
        var wp = mainCamera.ScreenToWorldPoint(Input.mousePosition); wp.z = 0f;
        var c = gridAdapter.WorldToCell(wp);
        s.OnNext(new PointerEvent(wp, c));
    }
}