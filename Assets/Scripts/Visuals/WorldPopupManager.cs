using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals{
    public class WorldPopupManager : MonoBehaviour
    {
        public static WorldPopupManager Instance { get; private set; }

        [SerializeField] private WorldPopup _popupPrefab;
        [SerializeField] private int _poolDefaultCapacity = 10;

        private Queue<WorldPopup> _popupPool;
        
        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
                return;
            }

            _popupPool = new(_poolDefaultCapacity);
        }

        private WorldPopup CreatePopup() {
            var popup = Instantiate(_popupPrefab, transform);
            popup.gameObject.SetActive(false);
            popup.SetDisableCallback(ReleasePopupToPool);
            return popup;
        }

        private void ReleasePopupToPool(WorldPopup popup) {
            popup.gameObject.SetActive(false);
            _popupPool.Enqueue(popup);
        }

        public void ShowPopup(string text, Color color, Vector3 position) {
            // Either grab one from the pool or create a new one
            WorldPopup popup = _popupPool.Count == 0 ? CreatePopup() : _popupPool.Dequeue();
            popup.SetText(text);
            popup.SetColor(color);
            popup.transform.position = position;
            popup.Show();
        }
    }
}