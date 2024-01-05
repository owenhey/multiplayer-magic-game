using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Helpers;
using UI;

namespace Drawing {
    public class SpellDrawingPopupManager : MonoBehaviour {
        public static SpellDrawingPopupManager Instance;

        [SerializeField] private RectTransform _spawnParent;
        [SerializeField] private SpellDrawingPopup _popupPrefab;

        void Start() {
            Instance = this;
        }
        
        public void ShowPopup(DrawingResults r) {
            var obj = Instantiate(_popupPrefab, _spawnParent);
            obj.Setup(r);
        }
        
        public void ShowPopup(string text) {
            var obj = Instantiate(_popupPrefab, _spawnParent);
            obj.Setup(text);
        }
    }
}