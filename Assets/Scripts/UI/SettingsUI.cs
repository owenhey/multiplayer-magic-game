using System;
using Cinemachine;
using Drawing;
using FishNet.Managing;
using Helpers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class SettingsUI : MonoBehaviour {
        [SerializeField] private PlayerSettings _playerSettings;
        [SerializeField] private PlayerSpells _playerSpells;
        [SerializeField] private PlayerStateManager _playerStateManager;
        [SerializeField] private PlayerCameraControls _playerCameraControls;

        [SerializeField] private GameObject _content;
        [SerializeField] private CinemachineVirtualCamera _settingsCamera;

        [Header("Additional Settings")] 
        [SerializeField] private GameObject _shooterOptions;
        [SerializeField] private GameObject _delayedIndicatorOptions;
        [SerializeField] private GameObject _shooterAreaHint;
        
        [Header("Buttons")] 
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _quickcastButton;
        [SerializeField] private Button _indicatorButton;
        [SerializeField] private Button _delayedIndicatorButton;
        [SerializeField] private Button _areaButton;
        [SerializeField] private Button _camStandardButton;
        [SerializeField] private Button _camShooterButton;
        [SerializeField] private Button _camMMOButton;
        [SerializeField] private Slider _spellSizeSlider;
        [SerializeField] private Slider _shooterMouseSensitivitySlider;
        [SerializeField] private Slider _autocastTimerSlider;
        [SerializeField] private Button _closeButton;

        public bool Active { get; private set; }

        private bool openedThisFrame = false;
        
        private void Awake() {
            Close();
            _resumeButton.onClick.AddListener(OnResumeClick);
            _quickcastButton.onClick.AddListener(OnQuickcastClick);
            _indicatorButton.onClick.AddListener(OnIndicatorClick);
            _areaButton.onClick.AddListener(OnAreaClick);
            _delayedIndicatorButton.onClick.AddListener(OnDelayedIndicatorClick);
            _camStandardButton.onClick.AddListener(OnStandardCameraClick);
            _camShooterButton.onClick.AddListener(OnThirdPersonShooterClick);
            _camMMOButton.onClick.AddListener(OnMMOClick);
            _spellSizeSlider.onValueChanged.AddListener(HandleDrawingCanvasSizeSlider);
            _shooterMouseSensitivitySlider.onValueChanged.AddListener(HandleShooterMouseSensSlider);
            _autocastTimerSlider.onValueChanged.AddListener(HandleAutocastTimerSlider);
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
            _settingsCamera.enabled = true;
            _content.SetActive(true);
            Active = true;
            _playerStateManager.AddState(PlayerState.InSettings);
            openedThisFrame = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UpdateAdditionalSettings();
        }

        private void Close() {
            _settingsCamera.enabled = false;
            _content.SetActive(false);
            Active = false;
            _playerStateManager.RemoveState(PlayerState.InSettings);
        }

        private void HandleDrawingCanvasSizeSlider(float delta) {
            float textSize = Misc.Remap(delta, 0, 1, 35, 90);
            float canvasSize = Misc.Remap(delta, 0, 1, 500, 1000);

            PlayerSettings.TextSize = textSize;
            PlayerSettings.CanvasSettingSize = (int)canvasSize;
        }
        
        private void HandleShooterMouseSensSlider(float delta) {
            DrawingManager.Instance.ShooterMouseSensativity = delta;
        }
        
        private void HandleAutocastTimerSlider(float t) {
            PlayerSpellIndicatorHandler.AUTOCAST_TIME = t;
        }

        private void UpdateAdditionalSettings() {
            _quickcastButton.GetComponent<Image>().color = _playerSpells.CastingType != SpellCastingType.Quickcast ? Color.white : new Color(.6f, .6f, 1.0f, 1.0f);
            _indicatorButton.GetComponent<Image>().color = _playerSpells.CastingType != SpellCastingType.Indicator ? Color.white : new Color(.6f, .6f, 1.0f, 1.0f);
            _delayedIndicatorButton.GetComponent<Image>().color = _playerSpells.CastingType != SpellCastingType.DelayedIndicator ? Color.white : new Color(.6f, .6f, 1.0f, 1.0f);
            _areaButton.GetComponent<Image>().color = _playerSpells.CastingType != SpellCastingType.Area ? Color.white : new Color(.6f, .6f, 1.0f, 1.0f);
            _camStandardButton.GetComponent<Image>().color = _playerCameraControls.CameraType != CameraMovementType.Standard ? Color.white : new Color(.6f, .6f, 1.0f, 1.0f);
            _camShooterButton.GetComponent<Image>().color = _playerCameraControls.CameraType != CameraMovementType.ThirdPersonShooter ? Color.white : new Color(.6f, .6f, 1.0f, 1.0f);
            _camMMOButton.GetComponent<Image>().color = _playerCameraControls.CameraType != CameraMovementType.MMO ? Color.white : new Color(.6f, .6f, 1.0f, 1.0f);
            
            
            _shooterOptions.SetActive(_playerCameraControls.CameraType == CameraMovementType.ThirdPersonShooter);
            _delayedIndicatorOptions.SetActive(_playerSpells.CastingType == SpellCastingType.DelayedIndicator);
            
            _shooterAreaHint.gameObject.SetActive(_playerSpells.CastingType == SpellCastingType.Area && 
                                                  _playerCameraControls.CameraType == CameraMovementType.ThirdPersonShooter);
        }

        private void OnResumeClick() {
            Close();
        }

        private void OnQuickcastClick() {
            _playerSpells.CastingType = SpellCastingType.Quickcast;
            UpdateAdditionalSettings();
        }

        private void OnIndicatorClick() {
            _playerSpells.CastingType = SpellCastingType.Indicator;
            UpdateAdditionalSettings();
        }

        private void OnAreaClick() {
            _playerSpells.CastingType = SpellCastingType.Area;
            UpdateAdditionalSettings();
        }
        
        private void OnDelayedIndicatorClick() {
            _playerSpells.CastingType = SpellCastingType.DelayedIndicator;
            UpdateAdditionalSettings();
        }
        
        private void OnStandardCameraClick() {
            _playerCameraControls.CameraType = CameraMovementType.Standard;
            UpdateAdditionalSettings();
        }

        private void OnThirdPersonShooterClick() {
            _playerCameraControls.CameraType = CameraMovementType.ThirdPersonShooter;
            UpdateAdditionalSettings();
        }

        private void OnMMOClick() {
            _playerCameraControls.CameraType = CameraMovementType.MMO;
            UpdateAdditionalSettings();
        }

        private void OnCloseGameClick() {
            var _networkManager = FindObjectOfType<NetworkManager>();
            _networkManager.ServerManager.StopConnection(true);
            SceneManager.LoadScene("Title");
        }
    }
}