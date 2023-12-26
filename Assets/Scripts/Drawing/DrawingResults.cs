using System;
using UnityEngine;
using Helpers;

public enum DrawingResultsAssessorTypes {
    SensitiveTriple
}
public class DrawingResults{
    public DrawingResults(bool completed, DefinedDrawing definedDrawing = null, float totalTime = 0, float averageDistance = 0, float percentDistanceError = 0, float newPercentDistanceError = 0, bool completedAllPoints = false, float score = 0) {
        Completed = completed;
        Drawing = definedDrawing;
        TotalTime = totalTime;
        AverageDistance = averageDistance;
        PercentDistanceError = percentDistanceError;
        CompletedAllPoints = completedAllPoints;
        NewPercentDistanceError = newPercentDistanceError;
    }

    #region Variables

    public readonly bool Completed;
    public readonly DefinedDrawing Drawing;
    public readonly float TotalTime;
    public readonly float AverageDistance;
    public readonly float PercentDistanceError;
    public readonly float NewPercentDistanceError;
    public readonly bool CompletedAllPoints;
    public float Score;

    #endregion

    public void AssessResults(DrawingResultsAssessorTypes type) {
        switch (type) {
            case DrawingResultsAssessorTypes.SensitiveTriple:
                Score = SensitiveTriple();
                break;
        }
    }

    private float SensitiveTriple() {
        return Misc.RemapClamp(AverageDistance, .01f, 0.07f, 1, 0);
    }

    public override string ToString() {
        return $"Completed all points: Score: {Score}, {CompletedAllPoints}, Total Time: {TotalTime}, Average Distance: {AverageDistance}, Percent Distance Error: {PercentDistanceError:P0}";
    }
}
