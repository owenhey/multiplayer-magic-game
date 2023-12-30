using UnityEngine;

namespace Drawing {
    public interface IDrawingAssessor {
        public void RegisterPoint(in Vector2 point);
        public DrawingResults Finish();

        public DefinedDrawing Target { get; }

        public void SetDebugDelegates(DebugPointDelegate temporaryDebug, DebugPointDelegate permanentDebug);
    }
}