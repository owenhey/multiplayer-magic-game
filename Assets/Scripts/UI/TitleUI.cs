using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class TitleUI : MonoBehaviour {
        public CanvasGroup _interactableCanvasGroup;
        public Image _fadeScreen;

        private void Awake() {
            if (Application.platform == RuntimePlatform.LinuxServer && Application.isEditor == false) {
                OnServerButton();
            }
        }

        public void OnClientButton() {
            Net.NetworkSettings.connectAsClient = true;
            LoadGame();
        }

        public void OnServerButton() {
            Net.NetworkSettings.connectAsClient = false;
            LoadGame();
        }

        private void LoadGame() {
            _interactableCanvasGroup.interactable = false;
            _fadeScreen.DOFade(1.0f, .5f).OnComplete(() => {
                SceneManager.LoadScene("SampleScene");
            });
        }

        public void OnQuit() {
            Application.Quit();
        }
    }
}