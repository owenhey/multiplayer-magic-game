using System;
using UnityEngine;
using Helpers;

public enum DrawingResultsAssessorTypes {
    SensitiveTriple,
    BasicAverageDistance,
    AverageDistanceWithAspectRatio
}
public class DrawingResults{
    public DrawingResults(bool completed, DefinedDrawing definedDrawing = null, float totalTime = 0, float averageDistance = 0, float percentDistanceError = 0, float newPercentDistanceError = 0, bool completedAllPoints = false, float score = 0, float aspectRatio = 1) {
        Completed = completed;
        Drawing = definedDrawing;
        TotalTime = totalTime;
        AverageDistance = averageDistance;
        PercentDistanceError = percentDistanceError;
        CompletedAllPoints = completedAllPoints;
        NewPercentDistanceError = newPercentDistanceError;
        AspectRatio = aspectRatio;
    }

    public readonly bool Completed;
    public readonly DefinedDrawing Drawing;
    public readonly float TotalTime;
    public readonly float AverageDistance;
    public readonly float PercentDistanceError;
    public readonly float NewPercentDistanceError;
    public readonly bool CompletedAllPoints;
    public float AspectRatio;
    
    public float Score;
    public Vector2 BottomLeftShapeSpace;
    public Vector2 BottomLeftScreenSpace; // Set by the drawing manager
    public Vector2 TopRightShapeSpace;
    public Vector2 TopRightScreenSpace; // Set by the drawing manager

    public void AssessResults(DrawingResultsAssessorTypes type) {
        switch (type) {
            case DrawingResultsAssessorTypes.SensitiveTriple:
                Score = SensitiveTriple();
                break;
            case DrawingResultsAssessorTypes.BasicAverageDistance:
                Score = BasicAverageDistance();
                break;
            case DrawingResultsAssessorTypes.AverageDistanceWithAspectRatio:
                Score = AvgDistanceWithAspectRatio();
                break;
        }
    }

    private float SensitiveTriple() {
        return Misc.RemapClamp(AverageDistance, .01f, 0.07f, 1, 0);
    }
    
    private float BasicAverageDistance() {
        float f = Misc.Remap(AverageDistance, Drawing.PerfectScoreThreshold, Drawing.FailCastThreshold, 1, 0);
        return Mathf.Min(1, f);
    }
    
    private float AvgDistanceWithAspectRatio() {
        float aspectRatioOff = Mathf.Abs(AspectRatio - Drawing.TargetAspectRatio) / Drawing.TargetAspectRatio;
        // Anything better than 10% off is perfect, otherwise, remaps to a 50% punishment
        float aspectRatioPunishment = Misc.RemapClamp(aspectRatioOff, .2f, .75f, 1.0f, .5f);
        // if (Math.Abs(Drawing.TargetAspectRatio - 1) > .001f) {
        //     Debug.Log($"Aspect ratio: {AspectRatio}");
        //     Debug.Log($"Aspect ratio off: {aspectRatioOff}");
        //     Debug.Log($"Aspect ratio punishment: {aspectRatioPunishment}");
        // }
        
        
        float f = Misc.Remap(AverageDistance, Drawing.PerfectScoreThreshold, Drawing.FailCastThreshold, 1, 0) * aspectRatioPunishment;
        return Mathf.Min(1, f);
    }

    public override string ToString() {
        return $"Completed all points: {CompletedAllPoints}. Score: {Score}, {CompletedAllPoints}, Total Time: {TotalTime}, Average Distance: {AverageDistance}, Percent Distance Error: {PercentDistanceError:P0}";
    }
}
