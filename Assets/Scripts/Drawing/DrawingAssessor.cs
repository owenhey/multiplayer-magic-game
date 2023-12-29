using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private BasicDrawingAssessor[] _assessors;

    public void HandleStartDraw(DefinedDrawing[] drawings) {
        _assessors = new BasicDrawingAssessor[drawings.Length];
        for (var index = 0; index < drawings.Length; index++) {
            var drawing = drawings[index];
            _assessors[index] = new BasicDrawingAssessor(drawing);
        }
    }

    public void HandleDraw(in Vector2 point) {
        foreach (var assessor in _assessors) {
            assessor.RegisterPoint(point);
        }
    }

    public DrawingResults HandleEndDraw() {
        // Get all the results
        DrawingResults[] results = new DrawingResults[_assessors.Length];
        for (int i = 0; i < _assessors.Length; i++) {
            results[i] = _assessors[i].Finish();
            results[i].AssessResults(_assessorType);
        }
        
        // Figure out which was closest
        float bestScore = 0;
        int bestIndex = -1;
        for (int i = 0; i < results.Length; i++) {
            if (results[i].Score >= bestScore) {
                bestScore = results[i].Score;
                bestIndex = i;
            }
        }
        
        return results[bestIndex];
    }

    public void SetDebug(DebugPointDelegate frameDebug, DebugPointDelegate indexDebug) {
        foreach (var item in _assessors) {
            item.SetDebugDelegates(frameDebug, indexDebug);
        }
    }

    public void AssessResults(DrawingResults results) {
        results.AssessResults(_assessorType);
    }
}
