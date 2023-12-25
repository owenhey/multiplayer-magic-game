using System;
using UnityEngine;
using Helpers;

public enum DrawingResultsAssessorTypes {
    SensitiveTriple
}
public class DrawingResults{
    public DrawingResults(DefinedDrawing definedDrawing, float totalTime, float averageDistance, float percentDistanceError, float newPercentDistanceError, bool completedAllPoints) {
        Drawing = definedDrawing;
        TotalTime = totalTime;
        AverageDistance = averageDistance;
        PercentDistanceError = percentDistanceError;
        CompletedAllPoints = completedAllPoints;
        NewPercentDistanceError = newPercentDistanceError;
    }

    #region Variables

    public readonly DefinedDrawing Drawing;
    public readonly float TotalTime;
    public readonly float AverageDistance;
    public readonly float PercentDistanceError;
    public float NewPercentDistanceError;
    public readonly bool CompletedAllPoints;

    #endregion

    public float AssessResults(DrawingResultsAssessorTypes type) {
        switch (type) {
            case DrawingResultsAssessorTypes.SensitiveTriple:
                return SensitiveTriple();
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private float SensitiveTriple() {
        return (Misc.RemapClamp(AverageDistance, .02f, 0.1f, 1, 0));
    }

    public override string ToString() {
        return $"Completed all points: {CompletedAllPoints}, Total Time: {TotalTime}, Average Distance: {AverageDistance}, Percent Distance Error: {PercentDistanceError:P0}";
    }
}
