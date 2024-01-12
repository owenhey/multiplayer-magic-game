using System;
using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using Spells;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class SpellUISlot : MonoBehaviour {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private LineUI _linePrefab;
        [SerializeField] private RectTransform _lineParent;

        private SpellInstance _spellInstance;

        public void Init(SpellInstance spellInstance) {
            if(_spellInstance != null) spellInstance.OnChange -= UpdateUI; // Unsub from the old event
            
            _spellInstance = spellInstance;
            
            DrawShape();
            _text.text = "";

            spellInstance.OnChange += UpdateUI;
            UpdateUI();
        }

        private void DrawShape() {
            var points = _spellInstance.SpellDefinition.Drawing.Points;
            for (int i = 0; i < points.Count - 1; i++) {
                Vector2 start = points[i].Vector * _lineParent.rect.width;
                Vector2 end = points[i + 1].Vector * _lineParent.rect.width;
                
                var line = Instantiate(_linePrefab, _lineParent);
                line.Setup(start, end, 10);
            }
        }

        private void UpdateUI() {
            if (_spellInstance.RemainingCooldown <= 0) {
                _text.text = "";
            }
            else {
                _text.text = _spellInstance.RemainingCooldown.ToString("0.0");
            }
        }
    }
}