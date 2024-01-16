using System;
using Helpers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI {
    public class SettingsUI : MonoBehaviour {
        [SerializeField] private PlayerSettings _playerSettings;
        [SerializeField] private PlayerSpells _playerSpells;
        [SerializeField] private PlayerStateManager _playerStateManager;
        [SerializeField] private PlayerCameraControls _playerCameraControls;

        [SerializeField] private GameObject _content;
        
        [Header("Buttons")] 
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _quickcastButton;
        [SerializeField] private Button _indicatorButton;
        [SerializeField] private Button _areaButton;
        [SerializeField] private Button _camStandardButton;
        [SerializeField] private Button _camShooterButton;
        [SerializeField] private Button _camMMOButton;
        [SerializeField] private Slider _spellSizeSlider;
        [SerializeField] private Button _closeButton;

        public bool Active { get; private set; }

        private bool openedThisFrame = false;
        
        private void Awake() {
            Close();
            _resumeButton.onClick.AddListener(OnResumeClick);
            _quickcastButton.onClick.AddListener(OnQuickcastClick);
            _indicatorButton.onClick.AddListener(OnIndicatorClick);
            _areaButton.onClick.AddListener(OnAreaClick);
            _camStandardButton.onClick.AddListener(OnStandardCameraClick);
            _camShooterButton.onClick.AddListener(OnThirdPersonShooterClick);
            _camMMOButton.onClick.AddListener(OnMMOClick);
            _spellSizeSlider.onValueChanged.AddListener(HandleSliderSet);
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

        private void HandleSliderSet(float delta) {
            float textSize = Misc.Remap(delta, 0, 1, 35, 90);
            float canvasSize = Misc.Remap(delta, 0, 1, 500, 1000);

            PlayerSettings.TextSize = textSize;
            PlayerSettings.CanvasSettingSize = (int)canvasSize;
        }

        private void OnResumeClick() {
            Close();
        }

        private void OnQuickcastClick() {
            _playerSpells.CastingType = SpellCastingType.Quickcast;
        }

        private void OnIndicatorClick() {
            _playerSpells.CastingType = SpellCastingType.QuickcastWithIndicator;
        }

        private void OnAreaClick() {
            _playerSpells.CastingType = SpellCastingType.Area;
        }
        
        private void OnStandardCameraClick() {
            _playerCameraControls.CameraType = CameraMovementType.Standard;
        }

        private void OnThirdPersonShooterClick() {
            _playerCameraControls.CameraType = CameraMovementType.ThirdPersonShooter;
        }

        private void OnMMOClick() {
            _playerCameraControls.CameraType = CameraMovementType.MMO;
        }

        private void OnCloseGameClick() {
            Application.Quit();
        }
    }
}