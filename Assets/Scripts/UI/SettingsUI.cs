using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class SettingsUI : MonoBehaviour {
        [SerializeField] private PlayerSettings _playerSettings;
        [SerializeField] private PlayerSpells _playerSpells;
        [SerializeField] private PlayerStateManager _playerStateManager;

        [SerializeField] private GameObject _content;
        
        [Header("Buttons")] 
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _quickcastButton;
        [SerializeField] private Button _indicatorButton;
        [SerializeField] private Button _areaButton;
        [SerializeField] private Button _closeButton;

        public bool Active { get; private set; }

        private bool openedThisFrame = false;
        
        private void Awake() {
            Close();
            _resumeButton.onClick.AddListener(OnResumeClick);
            _quickcastButton.onClick.AddListener(OnQuickcastClick);
            _indicatorButton.onClick.AddListener(OnIndicatorClick);
            _areaButton.onClick.AddListener(OnAreaClick);
            _closeButton.onClick.AddListener(OnCloseGameClick);
        }

        private void Update() {
            if (!Active) return;
            if (openedThisFrame) {
                openedThisFrame = false;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Close();
            }
        }

        private void OnEnable() {
            _playerSettings.OnOpenSettingsRequest += Open;
        }

        private void OnDisable() {
            _playerSettings.OnOpenSettingsRequest -= Open;
        }

        private void Open() {
            _content.SetActive(true);
            Active = true;
            _playerStateManager.AddState(PlayerState.InSettings);
            openedThisFrame = true;

            Cursor.lockState = CursorLockMode.None;
        }

        private void Close() {
            _content.SetActive(false);
            Active = false;
            _playerStateManager.RemoveState(PlayerState.InSettings);
        }

        private void OnResumeClick() {
            Close();
        }
        
        private void OnQuickcastClick() {
            
        }

        private void OnIndicatorClick() {
            
        }

        private void OnAreaClick() {
            
        }

        private void OnCloseGameClick() {
            Application.Quit();
        }
    }
}