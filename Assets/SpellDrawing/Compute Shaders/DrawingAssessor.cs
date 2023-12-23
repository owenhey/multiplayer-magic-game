using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drawing Assessor", menuName = "ScriptableObjects/Singletons/Drawing Assessor", order = 1)]
public class DrawingAssessor : ScriptableObject {
    [SerializeField] private DrawingResultsAssessorTypes _assessorType;

    public static DrawingAssessor Instance {
        get {
            if (_instance == null) {
                _instance = Resources.Load<DrawingAssessor>("Singletons/Drawing Assessor");
            }

            return _instance;
        }
    }

    private static DrawingAssessor _instance;

    private BasicDrawingAssessor _assessor;

    public void HandleStartDraw(DefinedDrawing drawing) {
        _assessor = new BasicDrawingAssessor(drawing);
    }

    public void HandleDraw(in Vector2 point) {
        _assessor.RegisterPoint(point);
    }

    public DrawingResults HandleEndDraw() {
        return _assessor.Finish();
    }

    public void SetDebug(DebugPointDelegate frameDebug, DebugPointDelegate indexDebug) {
        _assessor.SetDebugDelegates(frameDebug, indexDebug);
    }

    public float AssessResults(DrawingResults results) {
        return results.AssessResults(_assessorType);
    }
}
