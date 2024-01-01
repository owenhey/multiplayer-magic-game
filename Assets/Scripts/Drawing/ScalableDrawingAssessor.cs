using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drawing;
using Helpers;
using UnityEngine;

namespace Drawing{
    public class ScalableDrawingAssessor : IDrawingAssessor {
        public DefinedDrawing Target => _targetDrawing;

        private Vector2 _minBounds;
        private Vector2 _maxBounds;
        
        private List<Vector2> _points;

        private DefinedDrawing _targetDrawing;

        public ScalableDrawingAssessor(DefinedDrawing target) {
            _targetDrawing = target;
            _points = new(256);
            _minBounds = new Vector2(Mathf.Infinity, Mathf.Infinity);
            _maxBounds = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
        }
        
        public void RegisterPoint(in Vector2 point) {
            // Anytime a new point is registered, update the bounds
            AttemptUpdateBounds(point);
            _points.Add(point);
        }
        
        public void SetDebugDelegates(DebugPointDelegate temporaryDebug, DebugPointDelegate permanentDebug) {
            // Nothing for now
        }
        
        public DrawingResults Finish() {
            // First, translate the points we stored into space within the min and max bounds
            for (int i = 0; i < _points.Count; i++) {
                _points[i] = TranslatePoint(_points[i]);
            }

            // Then, loop through and find the best match for each point, going in order
            List<Vector2> closestPoints = new(_targetDrawing.Points.Count);
            int targetIndex = 0; // Target in the drawing we are currently going for
            float currentMinDistance = Mathf.Infinity;
            Vector2 currentMinPoint = Vector2.zero;
            for (int i = 0; i < _points.Count; i++) {
                Vector2 p = _points[i];
                float disToTarget = _targetDrawing.Points[targetIndex].SqDistance(p);
                
                if (disToTarget < currentMinDistance) {
                    currentMinDistance = disToTarget;
                    currentMinPoint = p;
                }

                float disToNext = targetIndex < (_targetDrawing.Points.Count - 1)
                    ? _targetDrawing.Points[targetIndex + 1].SqDistance(p)
                    : Mathf.Infinity;

                if (disToNext < disToTarget) {
                    targetIndex++;
                    closestPoints.Add(currentMinPoint);
                    currentMinDistance = Mathf.Infinity;
                    currentMinPoint = Vector2.zero;
                }
            }
            // Add the current smallest distance (the last one)
            closestPoints.Add(currentMinPoint);
            
            // Now, figure out average distance between points
            float average = 0;
            for (int i = 0; i < _targetDrawing.Points.Count; i++) {
                if(i < closestPoints.Count)
                    average += _targetDrawing.Points[i].Distance(closestPoints[i]);
                else {
                    average += 1f;
                }
            }
            average /= _targetDrawing.Points.Count;
            
            Debug.Log($"({_targetDrawing.name}) Target points count: " + _targetDrawing.Points.Count + ", Closest points count: " + closestPoints.Count +  $". {average}");

            return new DrawingResults(true, _targetDrawing, 0, average, 0, 0, true);
        }

        private Vector2 TranslatePoint(in Vector2 point) {
            float x = Misc.Remap(point.x, _minBounds.x, _maxBounds.x, 0, 1);
            float y = Misc.Remap(point.y, _minBounds.y, _maxBounds.y, 0, 1);
            return new Vector2(x, y);
        }

        private void AttemptUpdateBounds(in Vector2 point) {
            if (point.x < _minBounds.x) {
                _minBounds = new Vector2(point.x, _minBounds.y);
            }
            if (point.x > _maxBounds.x) {
                _maxBounds = new Vector2(point.x, _maxBounds.y);
            }
            if (point.y < _minBounds.y) {
                _minBounds = new Vector2(_minBounds.x, point.y);
            }
            if (point.y > _maxBounds.y) {
                _maxBounds = new Vector2(_maxBounds.x, point.y);
            }
        }
    }
}