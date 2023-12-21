using Interactable;
using PlayerScripts;
using UnityEngine;

namespace UI {
    public class InteractableUI : MonoBehaviour {
        [SerializeField] private PlayerInteract _playerInteract;
        [SerializeField] private Canvas _playerCanvas;
        [SerializeField] private InteractableLabelUI _indicator;

        [Header("Settings")] 
        [SerializeField] private float _horizontalOffset;

        private void OnEnable() {
            _playerInteract.OnInteractableChange += UpdateInteractable;
            UpdateInteractable();
        }

        private void OnDisable() {
            _playerInteract.OnInteractableChange -= UpdateInteractable;
        }

        private void Update() {
            // Calculates where on the canvas the mouse is, puts it to the right a bit
            RectTransform canvasRect = _playerCanvas.GetComponent<RectTransform>();
            // Convert the mouse position to a point in the canvas
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out Vector2 localPoint)){
                _indicator.SetPosition(localPoint + Vector2.right * _horizontalOffset); // A tad to the right
            }
        }

        private void UpdateInteractable() {
            IInteractable current = _playerInteract.CurrentInteractable;
            if (current == null) {
                _indicator.Hide();
                return;
            }
            _indicator.Show(current);
        }
    }
}