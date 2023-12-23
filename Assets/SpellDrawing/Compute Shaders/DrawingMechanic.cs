using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DrawingMechanic : MonoBehaviour
{
    public RectTransform canvasBounds;
    public RectTransform canvas;

    public ComputeShaderTest shader;
    [ReadOnly] public Vector2 bottomLeftCanvasSpace;
    [ReadOnly] public Vector2 topRightCanvasSpace;
    [ReadOnly] public Vector2 totalCanvasSize;

    public RectTransform overrideMouse;
    

    public System.Action<Vector2> OnDraw;
    public System.Action OnStartDraw;
    public System.Action<Vector2[], float> OnEndDraw;

    private List<Vector2> drawnPoints = new List<Vector2>(100);
    private float startTime;

    private bool drawingOnCanvas = false;


    private void Start(){
        Application.targetFrameRate = 256;
    }

    // Update is called once per frame
    void Update()
    {
        var pointOnCanvas = GetMouseValidity(Input.mousePosition);

        if(pointOnCanvas != null){
            if(Input.GetKeyDown(KeyCode.Mouse0)){
                OnStartDraw?.Invoke();
                drawnPoints.Clear();
                startTime = Time.time;
                drawingOnCanvas = true;
            }
            if(drawingOnCanvas && Input.GetKey(KeyCode.Mouse0)){
                Draw(pointOnCanvas.Value);
            }
        }
        if(drawingOnCanvas && Input.GetKeyUp(KeyCode.Mouse0)){
            SendPoints();
            drawingOnCanvas = false;
        }
    }

    private void SendPoints(){
        // loop backwards to find out last non-initialized one
        Vector2[] array = drawnPoints.Where(x=>x!=Vector2.zero).ToArray();
        float totalTime = Time.time - startTime;
        if(array.Length < 2) return;
        drawnPoints.Clear();
        OnEndDraw(array, totalTime);
    }

    private void Draw(Vector2 drawnPoint){
        drawnPoints.Add(drawnPoint);

        OnDraw?.Invoke(drawnPoint);
    }

    public void Clear() {
        shader.Clear();
    }

    private Vector2? GetMouseValidity(Vector2 mousePos) {
        RecalculateBounds();

        Vector3 mousePosition;
        if (overrideMouse != null) {
            mousePosition = overrideMouse.anchoredPosition;
        }
        else {
            mousePosition = Input.mousePosition;
        }

        mousePosition = new Vector3(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
        mousePosition = new Vector3(mousePosition.x * totalCanvasSize.x, mousePosition.y * totalCanvasSize.y, 0);

        float x = Remap(mousePosition.x, bottomLeftCanvasSpace.x, topRightCanvasSpace.x, 0, 1);
        float y = Remap(mousePosition.y, bottomLeftCanvasSpace.y, topRightCanvasSpace.y, 0, 1);

        Vector2 drawnPoint = drawnPoint = new Vector2(x, y);

        if(drawnPoint.x <= 0 || drawnPoint.x >= 1 || drawnPoint.y <= 0 || drawnPoint.y >= 1) return null;

        return drawnPoint;
    }

    private void RecalculateBounds() {
        totalCanvasSize = canvas.sizeDelta;
        var center = canvasBounds.anchoredPosition;
        var sizeRect = canvasBounds.sizeDelta;
        
        bottomLeftCanvasSpace = center - sizeRect * .5f;
        topRightCanvasSpace = center + sizeRect * .5f;
    }

    private float Remap(float value, float startLow, float startHigh, float endLow, float endHigh){
        if(value < startLow) value = startLow;
        if(value > startHigh) value = startHigh;
        return endLow + ((endHigh - endLow) / (startHigh - startLow)) * (value - startLow);
    }
}
