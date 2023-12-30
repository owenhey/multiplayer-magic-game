using System.Collections;
using System.Collections.Generic;
using Drawing;
using UnityEngine;

public class BasicDrawingAssessor : IDrawingAssessor{
    private DefinedDrawing _target;
    public DefinedDrawing Target => _target;

    private int _nextPointIndex;
    private float _minimumDistanceToCurrentPoint;
    private Vector2 _currentClosestPoint;
    private Vector2 _previousPoint;
    private float _startTime;
    private float _totalDistance;
    private float _newTotalDistance;
    private int _lastTargetPointIndex;

    private DebugPointDelegate _debugDelegate;
    private DebugPointDelegate _permanentDebug;

    private bool _startCountingDistance = false;

    private List<float> _completedPointDistances;
    private List<Vector2> _closestPoints;

    private int x = 0;
    private Vector2 previousX;
    
    public BasicDrawingAssessor(DefinedDrawing d) {
        _target = d;
        _nextPointIndex = 0;
        _totalDistance = 0;
        
        _minimumDistanceToCurrentPoint = Mathf.Infinity;
        _completedPointDistances = new(d.Points.Count);
        _closestPoints = new(d.Points.Count);
        _startTime = Time.time;
        _lastTargetPointIndex = d.Points.Count - 1;
    }

    public void SetDebugDelegates(DebugPointDelegate temporaryDebug, DebugPointDelegate permanentDebug){
        _debugDelegate = temporaryDebug;
        _permanentDebug = permanentDebug;
    }

    public void RegisterPoint(in Vector2 point) {
        if (_startCountingDistance) {
            _totalDistance += Vector2.Distance(point, _previousPoint);
        }
        else {
            previousX = point;
        }

        if (x++ == 10) {
            _newTotalDistance += Vector2.Distance(point, previousX);
            x = 0;
            previousX = point;
        }
        _startCountingDistance = true;
        _previousPoint = point;
        
        var targetPoint = _target.Points[_nextPointIndex];

        float disToTarget = targetPoint.SqDistance(point);
        
        if (_minimumDistanceToCurrentPoint > disToTarget) {
            _minimumDistanceToCurrentPoint = disToTarget;
            _currentClosestPoint = point;
        }

        if(_debugDelegate != null){
            _debugDelegate(_currentClosestPoint, _nextPointIndex);
        }

        // Stop looking for the next one if this is the last one
        bool lookForNext = _nextPointIndex != _lastTargetPointIndex;
        if (!lookForNext) return;
        
        var pointAfter = _target.Points[_nextPointIndex + 1];
        float disToNext = pointAfter.SqDistance(point);
        
        // Meaning jump to the next point
        if (disToNext < disToTarget) {
            if(_permanentDebug != null){
                _permanentDebug(_currentClosestPoint, _nextPointIndex);
            }

            _completedPointDistances.Add(Mathf.Sqrt(_minimumDistanceToCurrentPoint));
            _nextPointIndex++;
            _minimumDistanceToCurrentPoint = Mathf.Infinity;
            _closestPoints.Add(_currentClosestPoint);
        }
    }

    public DrawingResults Finish() {
        // Use the last point as the finishing point
        _completedPointDistances.Add(Mathf.Sqrt(_minimumDistanceToCurrentPoint));
        _closestPoints.Add(_currentClosestPoint);

        if(_permanentDebug != null){
            _permanentDebug(_currentClosestPoint, _lastTargetPointIndex);
        }

        float averageDistance = 0;
        for(int i = 0; i < _lastTargetPointIndex; i++){
            if(i < _completedPointDistances.Count){
                averageDistance += _completedPointDistances[i];
            }
            else{
                averageDistance += .15f;
            }
        }
        averageDistance /= _lastTargetPointIndex + 1;
        
        float totalTime = Time.time - _startTime;

        float distancePercent = Mathf.Abs(_totalDistance - _target.TotalDistance) / _target.TotalDistance;
        float newDistancePercent = Mathf.Abs(_newTotalDistance - _target.TotalDistance) / _target.TotalDistance;

        return new DrawingResults(true, _target, totalTime, averageDistance, distancePercent, newDistancePercent, _completedPointDistances.Count == _lastTargetPointIndex + 1);
    }
}