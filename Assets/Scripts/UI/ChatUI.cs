using System;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;
using TMPro;

namespace UI {
    public class ChatUI : MonoBehaviour {
        public bool Active { get; private set; } // Don't use enabled because we want it to update / listen to events, etc.

        [SerializeField] private int _maxDisplayed = 7;
        
        [SerializeField] private PlayerChat _playerChat;
        [SerializeField] private PlayerStateManager _playerStateManager;

        [SerializeField] private ChatMessageUI _chatMessagePrefab;

        [SerializeField] private RectTransform _chatMessageParent;
        [SerializeField] private GameObject _background;
        [SerializeField] private CanvasGroup _allContent;
        [SerializeField] private TMP_InputField _inputField;

        [SerializeField] private GameObject _enterToChatText;

        private LinkedList<ChatMessageUI> _shownChats = new();

        public void Update() {
            if (Active) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    SetChatActive(false);
                }
            }
        }
        
        private void HandleMessage(ChatMessage chatMessage) {
            var chatMessageUI = Instantiate(_chatMessagePrefab, _chatMessageParent);
            
            chatMessageUI.Init(chatMessage);
            chatMessageUI.SetDark(!Active);
            _shownChats.AddLast(chatMessageUI);
            if (_shownChats.Count > _maxDisplayed) {
                _shownChats.First.Value.FadeAndDestroy();
                _shownChats.RemoveFirst();
            }
        }

        private void SetChatActive(bool active) {
            Active = active;
            
            _enterToChatText.SetActive(!active);
            _background.SetActive(active);
            _allContent.alpha = active ? 1.0f : .35f;
            _allContent.interactable = _allContent.blocksRaycasts = active;
            
            foreach (var chatMessageUI in _shownChats) {
                chatMessageUI.SetDark(!active);
            }
            
            if (active) {
                _inputField.Select();
                _inputField.ActivateInputField();
                _playerStateManager.AddState(PlayerState.Chatting);
            }
            else {
                _playerStateManager.RemoveState(PlayerState.Chatting);
            }
        }

        private void HandleRequestChatFocus() {
            SetChatActive(true);
        }

        private void HandleMessageSubmit(string message) {
            _inputField.text = "";
            _inputField.caretPosition = 0;
            _inputField.Select();
            _inputField.ActivateInputField();
            
            message = message.Trim();
            if (string.IsNullOrEmpty(message)) return;
            
            _playerChat.SendMessageFromClient(message.Trim());
        }

        private void Awake() {
            SetChatActive(false);
            _inputField.onSubmit.AddListener(HandleMessageSubmit);
            HandleMessage(new ChatMessage(-1, "Help", "Type /name <new name> to set your name."));
            HandleMessage(new ChatMessage(-1, "Help", "Type /team <a,b,c,d> to set your team."));
        }
        
        private void OnEnable() {
            _playerChat.OnClientMessageReceived += HandleMessage;
            _playerChat.OnRequestChatFocus += HandleRequestChatFocus;
        }

        private void OnDisable() {
            _playerChat.OnClientMessageReceived -= HandleMessage;
            _playerChat.OnRequestChatFocus -= HandleRequestChatFocus;
        }
    }
}