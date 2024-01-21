using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

namespace Visuals {
    public interface IIndicator {
        void Init();
        Transform GetTransform();
        
        void SetPlayer(Player player);
        void SetPosition(Vector3 position);
        void SetSize(float size);
        void SetValid(bool valid);
        void SetActive(bool active);
    }
}