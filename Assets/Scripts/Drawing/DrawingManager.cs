using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.UI;

public delegate void DebugPointDelegate(in Vector2 v, int index);

public delegate void DrawShapeCallback(DrawingResults r);

public class DrawingManager : MonoBehaviour {
    public float size;
    
    [SerializeField] private DrawingMechanic _drawingMechanic;
    [SerializeField] [ReadOnly] private DefinedDrawing _targetDrawing;
    [SerializeField] private GameObject _content;

    [Header("Display")] 
        [SerializeField] private float _drawingSizeRelativeToParent = .76f;
        [SerializeField] private RectTransform _calculatedDrawingRT;
        [SerializeField] private RectTransform _displayParent;
        [SerializeField] private Image _guideImage;
        [SerializeField] private CanvasGroup _cg;

    [Header("Debugging")] 
        [SerializeField] public bool _debug = false;
        [SerializeField] private RectTransform _debugParent;
        [SerializeField] private RectTransform _showDrawingPointsParent;
        [SerializeField] private GameObject _debugDot;

    [Header("Drawings")] 
        [SerializeField] private DefinedDrawing _initialDrawing;
    
    private RectTransform _debugSlider;
    private Vector3 _fakeMousePos;

    public static DrawingManager Instance;
    private DrawShapeCallback _callback;


    private void Start() {
        Hide();
        Instance = this;
    }

    public void StartDrawing(DefinedDrawing drawing = null, DrawShapeCallback callback = null) {
        _targetDrawing = drawing;
        _cg.DOFade(1.0f, .15f).From(0);
        _fakeMousePos = _displayParent.anchoredPosition;
        _callback = null;
        _drawingMechanic.Clear();
        _callback = callback;
        ClearDebug();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            StartDrawing();
        }
    }

    public void Hide() {
        _content.SetActive(enabled);
    }

    private void Listen() {
        _drawingMechanic.OnStartDraw += OnStartDraw;
        _drawingMechanic.OnDraw += OnDraw;
        _drawingMechanic.OnEndDraw += OnEndDraw;
    }

    private void Mute() {
        _drawingMechanic.OnStartDraw -= OnStartDraw;
        _drawingMechanic.OnDraw -= OnDraw;
        _drawingMechanic.OnEndDraw -= OnEndDraw;
    }


    private void OnStartDraw() {
        ClearDebug();
        
        DrawingAssessor.Instance.HandleStartDraw(_targetDrawing);

        if (_debug) {
            _debugSlider = Instantiate(_debugDot, _debugParent).GetComponent<RectTransform>();
            DrawingAssessor.Instance.SetDebug(FrameDebug, IndexDebug);
        }
    }

    private void OnDraw(Vector2 point) {
        DrawingAssessor.Instance.HandleDraw(in point);
    }

    private void OnEndDraw(Vector2[] points, float time) {
        var results = DrawingAssessor.Instance.HandleEndDraw();
        var score = DrawingAssessor.Instance.AssessResults(results);
        Debug.Log(results);
        ShapeQualityPopupManager.Instance.ShowPopup(results, score);
        Hide();
        _callback?.Invoke(results);
    }
    
    private void FrameDebug(in Vector2 vec, int index){
        _debugSlider.anchoredPosition = _calculatedDrawingRT.sizeDelta * (vec - Vector2.one * .5f);
    }
    
    private void IndexDebug(in Vector2 vec, int index){
        var obj = Instantiate(_debugDot, _debugParent);
        var rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = _calculatedDrawingRT.sizeDelta * (vec - Vector2.one * .5f);
        obj.GetComponent<UnityEngine.UI.Image>().color = new Color(1, .6f, .45f, 1);
        obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (index + 1).ToString();

        rt.DOScale(Vector3.one * 1.5f, .2f * .5f).From(Vector3.zero).OnComplete(()=>{
            rt.transform.DOScale(Vector3.one, .2f * .5f);
        });
    }
    
    private void ClearDebug() {
        int childCount = _debugParent.childCount;
        for(int i = childCount - 1; i >= 0; i--){
            Destroy(_debugParent.GetChild(i).gameObject);
        }
    }
    
    private void ToggleShowShapePoints() {
        if (_showDrawingPointsParent.childCount > 0) {
            // Hide the dots
            int childCount = _showDrawingPointsParent.childCount;
            for(int i = childCount - 1; i >= 0; i--){
                Destroy(_showDrawingPointsParent.GetChild(i).gameObject);
            }
        }
        else {
            // Show the dots
            for (int i = 0; i < _targetDrawing.Points.Count; i++) {
                var obj = Instantiate(_debugDot, _showDrawingPointsParent);
                var rt = obj.GetComponent<RectTransform>();
                rt.anchoredPosition = _calculatedDrawingRT.sizeDelta * (_targetDrawing.Points[i].Vector - Vector2.one * .5f);
            }
        }
    }

    public void SetSize(float sizeT) {
        Debug.Log("Size t: " + sizeT);
        _displayParent.sizeDelta = new Vector2(sizeT, sizeT);
        _calculatedDrawingRT.sizeDelta = new Vector2(sizeT, sizeT) * _drawingSizeRelativeToParent;
    }
    
    private void OnEnable() {
        Listen();
    }
    
    private void OnDisable() {
        Mute();
    }
}
