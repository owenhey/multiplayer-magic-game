using System;
using DG.Tweening;
using Helpers;
using PlayerScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ChatMessageUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _textField;

        public void Init(ChatMessage message) {
            DateTime localTime = message.SendTimeUTC.ToLocalTime();
            string localTimeString = localTime.ToString("hh:mm tt");
            _textField.text = $"[{localTimeString}] [{message.SenderName}]: {message.Message}";

            var parent = transform.parent;
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent.RT());
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent.parent.RT());
        }

        public void FadeAndDestroy() {
            _textField.DOFade(0, 1.0f).OnComplete(()=>Destroy(gameObject));
        }

        public void SetDark(bool dark) {
            _textField.color = dark ? Color.black : Color.white;
        }
    }
}