using System;
using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using TMPro;
using UnityEngine;

namespace UI{
    public class PlayerHealthUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private PlayerStats _playerStats;

        private void UpdateHealth(int health) {
            _text.text = $"{health} / {_playerStats.MaxHealth}";
        }

        private void UpdateHealth() {
            UpdateHealth(_playerStats.CurrentHealth);
        }

        private void OnEnable() {
            UpdateHealth();
            _playerStats.OnHealthChange += UpdateHealth;
        }

        private void OnDisable() {
            _playerStats.OnHealthChange -= UpdateHealth;
        }
    }
}