using FishNet.Object;
using UnityEngine;
using PlayerScripts;

namespace Interactable{
    public class ClientInteractable : MonoBehaviour, IInteractable {
        [Header("Display")] 
            [SerializeField] private int _order;
            [SerializeField] private Sprite _icon;
            [SerializeField] private string _displayText;
        
        public int Order => _order;
        public Sprite Icon => _icon;
        public string DisplayText => _displayText;
        
        public void ClientInteract(Player player) {
            Debug.Log("Interacted with: " + _displayText);
        }
    }
}