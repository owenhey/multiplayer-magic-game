using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using PlayerScripts;
using UnityEngine.UI;

namespace Drawing {
    public delegate void DebugPointDelegate(in Vector2 v, int index);

    public delegate void DrawShapeCallback(DrawingResults r);

    public class DrawingManager : MonoBehaviour {
        public float size;

        [SerializeField] private DrawingMechanic _drawingMechanic;
        [SerializeField] [ReadOnly] private DefinedDrawing _targetDrawing;
        [SerializeField] private GameObject _content;

        [Header("Display")] [SerializeField] private RectTransform _calculatedDrawingRT;
        [SerializeField] private RectTransform _displayParent;
        [SerializeField] private Image _guideImage;
        [SerializeField] private Image _circleImage;
        [SerializeField] private CanvasGroup _cg;
        [SerializeField] private RectTransform[] _rts;

        [Header("Debugging")] [SerializeField] public bool _debug = false;
        [SerializeField] private RectTransform _debugParent;
        [SerializeField] private RectTransform _showDrawingPointsParent;
        [SerializeField] private GameObject _debugDot;

        private RectTransform _debugSlider;
        private Vector3 _fakeMousePos;

        public static DrawingManager Instance;
        private DrawShapeCallback _callback;

        private bool _open;
        public bool Open => _open;
        private bool _offsetShapes;

        private void Start() {
            Hide();
            Instance = this;
        }

        public void StartDrawing(DefinedDrawing drawing = null, DrawShapeCallback callback = null,
            bool offsetShapes = false) {
            _cg.DOKill();
            _cg.alpha = 1;
            _cg.interactable = true;
            
            if (drawing != null) {
                SetSize(100);
                _targetDrawing = drawing;
                // Set the drawing on the mouse
                _guideImage.enabled = true;
                _guideImage.sprite = drawing.HelperImage;
                Vector2 firstPointOffset =
                    _targetDrawing.GetStartingPointOffsetInPixels(_calculatedDrawingRT.sizeDelta.x);
                PositionDrawing((Vector2)Input.mousePosition - firstPointOffset);
                // PositionDrawing((Vector2)Input.mousePosition);
                _circleImage.enabled = false;
            }
            else if (drawing == null && !offsetShapes) {
                _targetDrawing = null;
                // SetSize(100);
                // In this case, just position it in the center of the circle. Maybe show the indicator here?
                _guideImage.enabled = false;
                PositionDrawing((Vector2)Input.mousePosition);
                _circleImage.enabled = true;
            }

            else {
                _targetDrawing = null;
                // SetSize(1000);
                SetSize(PlayerSettings.CanvasSettingSize);
                PositionDrawing((Vector2)Input.mousePosition);
                // PLACEHOLDER FOR THE INSTANT CAST METHOD
                _guideImage.enabled = false;
                _circleImage.enabled = false;
                _drawingMechanic.ForceStartDraw();
            }

            _offsetShapes = offsetShapes;
            _content.SetActive(true);
            _cg.DOFade(1.0f, .15f).From(0);
            _drawingMechanic.Clear();
            _callback = callback;
            _open = true;

            ClearDebug();
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
            ClearDebug();

            if (_targetDrawing != null) {
                DrawingAssessor.Instance.HandleStartDraw(new[] { _targetDrawing });
            }
            else {
                // Grab all spells
                var spells = Player.LocalPlayer.PlayerReferences.PlayerSpells.GetAllEquippedSpells();
                DrawingAssessor.Instance.HandleStartDraw(spells.Select(x => x.Drawing).ToArray());
            }

            if (_debug) {
                _debugSlider = Instantiate(_debugDot, _debugParent).GetComponent<RectTransform>();
                DrawingAssessor.Instance.SetDebug(FrameDebug, IndexDebug);
            }
        }

        private void OnDraw(Vector2 point) {
            if (_offsetShapes) {
                DrawingAssessor.Instance.HandleDrawTranslated(in point, _calculatedDrawingRT.sizeDelta.x);
            }
            else {
                DrawingAssessor.Instance.HandleDraw(in point);
            }

        }

        private void OnEndDraw(Vector2[] points, float time) {
            var results = DrawingAssessor.Instance.HandleEndDraw();
        
            Finish(results);
        }

        // Can be called either from Cancel or OnEndDraw
        private void Finish(DrawingResults results) {
            _cg.interactable = false;
            _open = false;
            _cg.DOFade(0, .5f).SetDelay(.25f).OnComplete(Hide);
            _callback?.Invoke(results);
        }

        private void FrameDebug(in Vector2 vec, int index) {
            if (!_debugSlider) {
                _debugSlider = Instantiate(_debugDot, _debugParent).GetComponent<RectTransform>();
            }

            _debugSlider.anchoredPosition = _calculatedDrawingRT.sizeDelta * (vec - Vector2.one * .5f);
        }

        private void IndexDebug(in Vector2 vec, int index) {
            var obj = Instantiate(_debugDot, _debugParent);
            var rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = _calculatedDrawingRT.sizeDelta * (vec - Vector2.one * .5f);
            obj.GetComponent<UnityEngine.UI.Image>().color = new Color(1, .6f, .45f, 1);
            obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (index + 1).ToString();

            rt.DOScale(Vector3.one * 1.5f, .2f * .5f).From(Vector3.zero).OnComplete(() => {
                rt.transform.DOScale(Vector3.one, .2f * .5f);
            });
        }

        private void ClearDebug() {
            int childCount = _debugParent.childCount;
            for (int i = childCount - 1; i >= 0; i--) {
                Destroy(_debugParent.GetChild(i).gameObject);
            }
        }

        public void SetSize(float sizeT) {
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