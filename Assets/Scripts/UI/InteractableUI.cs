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
        
        private IInteractable _current;

        private void OnEnable() {
            _playerInteract.OnInteractableChange += UpdateInteractable;
            UpdateInteractable();
        }

        private void OnDisable() {
            _playerInteract.OnInteractableChange -= UpdateInteractable;
        }

        private void Update() {
            if (_indicator.Showing == false) return; // No need to do this unless there is something to show
            
            // Calculates where on the canvas the mouse is, puts it to the right a bit
            RectTransform canvasRect = _playerCanvas.GetComponent<RectTransform>();
            // Convert the mouse position to a point in the canvas
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out Vector2 localPoint)){
                _indicator.SetPosition(localPoint); // A tad to the right
            }
        }

        private void UpdateInteractable() {
            _current = _playerInteract.CurrentInteractable;
            if (_current == null) {
                _indicator.Hide();
                return;
            }
            _indicator.Show(_current);
        }
    }
}