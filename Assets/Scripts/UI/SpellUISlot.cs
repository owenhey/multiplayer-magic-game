using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
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
        [SerializeField] private HovererUI _hoverer;

        private SpellInstance _spellInstance;

        public void Init(SpellInstance spellInstance) {
            if(_spellInstance != null) spellInstance.OnChange -= UpdateUI; // Unsub from the old event
            
            _spellInstance = spellInstance;
            
            DrawShape();
            _text.text = "";
            _hoverer.SetText(spellInstance.SpellDefinition.SpellName);

            spellInstance.OnChange += UpdateUI;
            UpdateUI();
        }

        private void DrawShape() {
            _lineParent.transform.ForEachChild(x=>Destroy(x.gameObject));
            
            var points = _spellInstance.SpellDefinition.Drawing.Points;
            for (int i = 0; i < points.Count - 1; i++) {
                var rect = _lineParent.rect;
                Vector2 start = TranslatePoint(points[i].Vector, .75f) * ((rect.width + .125f) * .75f);
                Vector2 end = TranslatePoint(points[i + 1].Vector, .75f) * ((rect.width + .125f) * .75f);
                
                var line = Instantiate(_linePrefab, _lineParent);
                line.Setup(start, end, 10);
                if(i == 0) line.SetColor(Color.green);
            }
        }

        private Vector2 TranslatePoint(Vector2 point, float factor) {
            point -= new Vector2(.5f, .5f);
            point *= factor;
            return point;
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