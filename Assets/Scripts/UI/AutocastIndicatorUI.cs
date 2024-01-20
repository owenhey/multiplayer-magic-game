using System;
using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class AutocastIndicatorUI : MonoBehaviour {
        [SerializeField] private Canvas _playerCanvas;
        [SerializeField] private PlayerSpellIndicatorHandler _indicatorHandler;
        [SerializeField] private RectTransform _sprite;

        private bool _showing;
        
        private void Awake() {
            _indicatorHandler.OnAutocastSet += HandleAutocastSet;
            _indicatorHandler.OnAutocastTick += HandleAutocastTick;
            
        }

        private void Update() {
            if (_showing == false) return; // No need to do this unless there is something to show
            
            // Calculates where on the canvas the mouse is, puts it to the right a bit
            RectTransform canvasRect = _playerCanvas.GetComponent<RectTransform>();
            // Convert the mouse position to a point in the canvas
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out Vector2 localPoint)) {
                _sprite.anchoredPosition = localPoint;
            }
        }

        private void HandleAutocastTick(float f) {
            _sprite.transform.localScale = Vector3.one * Mathf.Lerp(1, 0, f);
        }

        private void HandleAutocastSet(bool b) {
            _sprite.gameObject.SetActive(b);
            _showing = b;
        }
    }
}
