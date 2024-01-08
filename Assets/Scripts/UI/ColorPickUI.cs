using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

namespace UI {
    public class ColorPickUI : MonoBehaviour {
        [SerializeField] private PlayerModel _playerModel;
        [SerializeField] private GameObject _content;
        [SerializeField] private List<ColorButtonUI> _colorButtons;
        [SerializeField] private GameObject _openButton;
        [SerializeField] private GameObject _closeButton;

        private void Start() {
            Close();
            for (int i = 0; i < _colorButtons.Count; i++) {
                int x = i;
                _colorButtons[i].Button.onClick.AddListener(() => {
                    _playerModel.SetColorFromClient(_colorButtons[x].Color);
                });
            }            
        }

        public void Open() {
            _content.SetActive(true);
            _openButton.SetActive(false);
            _closeButton.SetActive(true);
        }

        public void Close() {
            _content.SetActive(false);
            _openButton.SetActive(true);
            _closeButton.SetActive(false);
        }
    }
}