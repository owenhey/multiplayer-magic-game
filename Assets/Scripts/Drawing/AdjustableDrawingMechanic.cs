using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class AdjustableDrawingMechanic : MonoBehaviour, IDragHandler, IBeginDragHandler {
    private RectTransform rt;
    public DrawingManager drawer;
    public RectTransform drawingMechanic;
    public RectTransform drawingArea;
    public RectTransform canvas;

    public Vector2 offset;
    private Vector2 startDragPoint;
    private Vector2 startAnchoredPosPoint;

    public void Start() {
        rt = GetComponent<RectTransform>();
        offset = drawingMechanic.anchoredPosition - rt.anchoredPosition;
    }

    public void OnMove() {
        Vector2 mousePos = GetMousePosInPixels();

        drawingMechanic.anchoredPosition = startAnchoredPosPoint + (mousePos - startDragPoint);
        drawingArea.anchoredPosition = startAnchoredPosPoint + (mousePos - startDragPoint);
    }

    public void HandleSlider(float x) {
        drawer.SetSize(x * 1500.0f);
    }

    private Vector2 GetMousePosInPixels() {
        Vector2 mousePosition = Input.mousePosition;

        var totalCanvasSize = canvas.sizeDelta;
        
        mousePosition = new Vector3(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
        mousePosition = new Vector3(mousePosition.x * totalCanvasSize.x, mousePosition.y * totalCanvasSize.y, 0);

        return mousePosition;
    }

    public void OnDrag(PointerEventData eventData) {
        OnMove();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        startDragPoint = GetMousePosInPixels();
        startAnchoredPosPoint = drawingMechanic.anchoredPosition;
    }
}
