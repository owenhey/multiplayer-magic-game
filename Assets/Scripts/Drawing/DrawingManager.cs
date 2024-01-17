using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using DG.Tweening;
using Helpers;
using PlayerScripts;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Drawing {
    public delegate void DebugPointDelegate(in Vector2 v, int index);

    public delegate void DrawShapeCallback(DrawingResults r);

    public class DrawingManager : MonoBehaviour {

        [SerializeField] private DrawingMechanic _drawingMechanic;
        [SerializeField] private GameObject _content;

        [Header("Display")] 
        [SerializeField] private RectTransform _calculatedDrawingRT;
        [SerializeField] private CanvasGroup _cg;
        [SerializeField] private RectTransform[] _rts;
        [SerializeField] private RectTransform _displayRT;

        private RectTransform _debugSlider;

        public static DrawingManager Instance;
        private DrawShapeCallback _callback;

        private bool _open;
        public bool Open => _open;
        private float size;
        
        public static System.Action<Vector2> OnTranslatedDraw;
        public static System.Action OnTranslatedStartDraw;
        public static System.Action<Vector2[], float> OnTranslatedEndDraw;

        public static readonly Vector2 HALF = new Vector2(.5f, .5f);

        private CameraMovementType _camType;
        

        private void Start() {
            Hide();
            Instance = this;
            _drawingMechanic.Init();
        }

        public void StartDrawing(CameraMovementType camMoveType, DrawShapeCallback callback) {
            _camType = camMoveType;
            switch (camMoveType) {
                case CameraMovementType.Standard:
                    _drawingMechanic.SetUseVirtualMouse(false);
                    PositionDrawing((Vector2)Input.mousePosition);
                    break;
                case CameraMovementType.ThirdPersonShooter:
                    Vector2 centerOfScreen = HALF * new Vector2(Screen.width, Screen.height);
                    _drawingMechanic.SetUseVirtualMouse(true, centerOfScreen);
                    PositionDrawing(centerOfScreen);
                    PlayerCameraControls.MouseSensativity = .05f;
                    break;
                case CameraMovementType.MMO:
                    PositionDrawing((Vector2)Input.mousePosition);
                    break;
            }
            ResetDrawingCanvas(callback);
        }

        private void ResetDrawingCanvas(DrawShapeCallback callback) {
            SetSize(PlayerSettings.CanvasSettingSize);
            _content.SetActive(true);
            _drawingMechanic.Clear();
            _callback = callback;
            _open = true;
            _drawingMechanic.ForceStartDraw();
        }

        public void Hide() {
            _content.SetActive(false);
            _open = false;
        }

        private void Update() {
            if (_open && Input.GetKeyDown(KeyCode.Mouse1)) {
                CancelDrawing();
            }
        }

        private void CancelDrawing() {
            var cancelledResults = new DrawingResults(false);
            Finish(cancelledResults);
        }

        private void PositionDrawing(Vector2 position) {
            foreach (var rectTransform in _rts) {
                rectTransform.anchoredPosition = position;
            }
        }

        private void OnStartDraw() {
            // Grab all spells
            var spells = Player.LocalPlayer.PlayerReferences.PlayerSpells.GetAllEquippedSpells();
            DrawingAssessor.Instance.HandleStartDraw(spells.Select(x => x.Drawing).ToArray());
            
            OnTranslatedStartDraw?.Invoke();
        }

        private void OnDraw(Vector2 point) {
            DrawingAssessor.Instance.HandleDrawTranslated(in point, _calculatedDrawingRT.sizeDelta.x);
            if (_camType == CameraMovementType.ThirdPersonShooter) {
                OnTranslatedDraw?.Invoke(point);
                Vector2 screen = new Vector2(Screen.width, Screen.height);
                Vector2 halfScreen = screen * .5f;
                // translate point to pixel coordinates
                Vector2 pixelLoc = (HALF - point) * size;
                Vector2 centerOfScreenInPixels = HALF * screen;
                


                _displayRT.anchoredPosition = halfScreen + pixelLoc;
            }
            else {
                // NORMAL
                OnTranslatedDraw?.Invoke(point);
            }
        }

        private void OnEndDraw(Vector2[] points, float time) {
            var results = DrawingAssessor.Instance.HandleEndDraw();
            Finish(results);
            
            PlayerCameraControls.MouseSensativity = 1.0f;
            
            OnTranslatedEndDraw?.Invoke(points, time);
        }

        // Can be called either from Cancel or OnEndDraw
        private void Finish(DrawingResults results) {
            Hide();
            _callback?.Invoke(results);
        }

        public void SetSize(float sizeT) {
            size = sizeT;
            foreach (var item in _rts) {
                item.sizeDelta = new Vector2(sizeT, sizeT);
            }
        }

        private void OnEnable() {
            _drawingMechanic.OnStartDraw += OnStartDraw;
            _drawingMechanic.OnDraw += OnDraw;
            _drawingMechanic.OnEndDraw += OnEndDraw;
        }

        private void OnDisable() {
            _drawingMechanic.OnStartDraw -= OnStartDraw;
            _drawingMechanic.OnDraw -= OnDraw;
            _drawingMechanic.OnEndDraw -= OnEndDraw;
        }
    }
}