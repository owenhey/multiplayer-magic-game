using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using PlayerScripts;

namespace Interactable{
    public class NetworkedInteractable : NetworkBehaviour, IInteractable{
        [Header("Display")] 
            [SerializeField] private int _order;
            [SerializeField] private Sprite _icon;
            [SerializeField] private string _displayText;
            [SerializeField] private float _interactDistance = 3;
            
        public int Order => _order;
        public Sprite Icon => _icon;
        public string DisplayText => _displayText;
        public float InteractDistance => _interactDistance;
        
        public void ClientInteract(Player player) {
            Debug.Log("Client calls: " + _displayText);
            ServerRpcHandleClientInteraction();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ServerRpcHandleClientInteraction(NetworkConnection interactingPlayer = null) {
            Debug.Log("Server calls: " + _displayText);
        }
    }
}