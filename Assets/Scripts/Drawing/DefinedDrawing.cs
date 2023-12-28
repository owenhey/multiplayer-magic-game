using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Defined Drawing", menuName = "ScriptableObjects/Drawing", order = 1)]
public class DefinedDrawing : ScriptableObject
{
    [System.Serializable]
    public class DrawingPoint{
        [Range(0, 1)] public float X;
        [Range(0, 1)] public float Y;

        [Range(0, 1)] public float time;

        public Vector2 Vector => new Vector2(X,Y);

        public float Distance(Vector2 input){
            return Vector2.Distance(Vector, input);
        }

        public float SqDistance(Vector2 input) {
            return (input - Vector).SqrMagnitude();
        }
    }

    public List<DrawingPoint> Points;
    
    [Space(20)]
    public float TotalDistance;
    [ReadOnly] public float CalculatedDistance;
    [Range(0, 2)] public float TimeTarget;

    [Space(20)] 
    public Sprite HelperImage;

    public Vector2 GetStartingPointOffsetInPixels(float canvasSize) {
        return canvasSize * (Points[0].Vector - Vector2.one * .5f);
    }
    
    public Vector2 GetCenterPointInPixels(float canvasSize) {
        return canvasSize * Vector2.one * .5f;
    }

    private void OnValidate() {
        #if UNITY_EDITOR
        CalculatedDistance = 0;
        for(int i = 0; i < Points.Count - 1; i++){
            CalculatedDistance += Points[i].Distance(Points[i + 1].Vector);
        }
        #endif
    }
}
