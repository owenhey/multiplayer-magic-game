using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Helpers;

public class ShapeUI : MonoBehaviour
{
    public List<DefinedDrawing> drawingShapes;
    public int shapeIndex = 0;

    [Header("Refs")]
    public DrawingManager drawingManager;
    public TextMeshProUGUI shapeText;

    private void Start(){
        SetShape(0);
        drawingManager.SetSize(700);
    }

    public void SizeSlider(float x) {
        float size = Misc.Remap(x, 0, 1, 0, 10000);
        drawingManager.SetSize(size);
    }

    public void SetShape(int index){
        index %= drawingShapes.Count;
        drawingManager.SetTargetDrawing(drawingShapes[index]);
        shapeText.text = drawingShapes[index].name;
    }

    public void OnShapeButtonPress(){
        shapeIndex++;
        SetShape(shapeIndex);
    }
}
