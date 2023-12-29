using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Object;
using PlayerScripts;
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
        [SerializeField] private RectTransform[] _rts;

    [Header("Debugging")] 
        [SerializeField] public bool _debug = false;
        [SerializeField] private RectTransform _debugParent;
        [SerializeField] private RectTransform _showDrawingPointsParent;
        [SerializeField] private GameObject _debugDot;
    
    private RectTransform _debugSlider;
    private Vector3 _fakeMousePos;

    public static DrawingManager Instance;
    private DrawShapeCallback _callback;

    private bool _open;

    private void Start() {
        Hide();
        Instance = this;
    }

    public void StartDrawing(DefinedDrawing drawing = null, DrawShapeCallback callback = null) {
        if (drawing != null) {
            _targetDrawing = drawing;
            // Set the drawing on the mouse
            _guideImage.enabled = true;
            _guideImage.sprite = drawing.HelperImage;
            Vector2 firstPointOffset = _targetDrawing.GetStartingPointOffsetInPixels(_calculatedDrawingRT.sizeDelta.x);
            PositionDrawing((Vector2)Input.mousePosition - firstPointOffset);
        }
        else {
            // In this case, just position it in the center of the circle. Maybe show the indicator here?
            _guideImage.enabled = false;
            PositionDrawing((Vector2)Input.mousePosition);
        }
        
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
            DrawingAssessor.Instance.HandleStartDraw(new []{_targetDrawing});
        }
        else {
            // Grab all spells
            var spells = Player.LocalPlayer.PlayerReferences.PlayerSpells.GetOffCooldownSpells();
            DrawingAssessor.Instance.HandleStartDraw(spells.Select(x=>x.Drawing).ToArray());
        }

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
        DrawingAssessor.Instance.AssessResults(results);
        Finish(results);
    }

    // Can be called either from Cancel or OnEndDraw
    private void Finish(DrawingResults results) {
        Debug.Log(results);
        // ShapeQualityPopupManager.Instance.ShowPopup(results, score);
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

    public void SetSize(float sizeT) {
        Debug.Log("Size t: " + sizeT);
        _displayParent.sizeDelta = new Vector2(sizeT, sizeT);
        _calculatedDrawingRT.sizeDelta = new Vector2(sizeT, sizeT) * _drawingSizeRelativeToParent;
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
