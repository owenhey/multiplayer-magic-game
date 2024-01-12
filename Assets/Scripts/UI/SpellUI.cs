using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;
using Spells;
using PlayerScripts;
using UnityEngine.UI;

namespace UI{
    public class SpellUI : MonoBehaviour {
        [SerializeField] private PlayerSpells _spells;
        
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _content;

        [SerializeField] private List<SpellUISlot> _spellUISlots;

        private void Start() {
            if (InstanceFinder.IsClient == false) {
                Destroy(gameObject);
                return;
            }
            _openButton.onClick.AddListener(Open);
            _closeButton.onClick.AddListener(Close);

            Close();
            TryInit();
        }

        private void TryInit() {
            var spellInstances = _spells.SpellInstances;
            if (spellInstances == null) {
                StartCoroutine(DelayedInit());
                return;
            }

            Init();
        }
        
        private IEnumerator DelayedInit() {
            yield return new WaitUntil(()=>_spells.SpellInstances != null);
            Init();
        }

        private void Init() {
            var spellInstances = _spells.SpellInstances;
            for (int i = 0; i < _spellUISlots.Count; i++) {
                if (i < spellInstances.Count) {
                    _spellUISlots[i].Init(spellInstances[i]);
                    _spellUISlots[i].gameObject.SetActive(true);
                }
                else {
                    _spellUISlots[i].gameObject.SetActive(false);
                }
            }
        }
        
        public void Open() {
            _openButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(true);
            _content.SetActive(true);
        }

        private void Close() {
            _openButton.gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(false);
            _content.SetActive(false);
        }
    }
}