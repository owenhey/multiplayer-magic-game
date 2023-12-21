using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerScripts;

namespace Interactable{
    public interface IInteractable {
        int Order { get; }
        Sprite Icon { get; }
        string DisplayText { get; }
        
        float InteractDistance { get; }

        /// <summary>
        /// Called from and only on the interacting client to initiate the interaction
        /// </summary>
        void ClientInteract(Player player);
    }
}