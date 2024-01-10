using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers {
    public class UIConsoleLogger : MonoBehaviour {
        [SerializeField] private bool _showStackTrace;
        [SerializeField] private RectTransform _consoleLineParent;
        [SerializeField] private GameObject _consoleLinePrefab;
        [SerializeField] private GameObject _consoleObj;
        [SerializeField] private GameObject _closeButton;
        [SerializeField] private GameObject _openButton;

        private string _output;
        private string _stackTrace;
        private string _currentLog;

        private void Awake() {
            // if (Application.isEditor) {
            //     Destroy(this.gameObject);
            //     return;
            // }

            _consoleObj.SetActive(false);
            _openButton.SetActive(true);
            _closeButton.SetActive(false);
        }

        public void ToggleShow() {
            _consoleObj.SetActive(!_consoleObj.activeInHierarchy);

            bool open = _consoleObj.activeInHierarchy;
            
            _closeButton.SetActive(open);
            _openButton.SetActive(!open);
        }

        private void Log(string logString, string stackTrace, LogType type) {
            StringBuilder myString = new();
            myString.Append(logString);
            if (_showStackTrace) {
                myString.Append("<br>");
                myString.Append(stackTrace);
            }

            var newLine = Instantiate(_consoleLinePrefab, _consoleLineParent);
            newLine.GetComponentInChildren<TextMeshProUGUI>().text = myString.ToString();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_consoleLineParent);
        }

        private void OnEnable() {
            Application.logMessageReceived += Log;
        }

        private void OnDisable() {
            Application.logMessageReceived -= Log;
        }
    }
}