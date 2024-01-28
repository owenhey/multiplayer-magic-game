using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

namespace Visuals {
    public interface IIndicator {
        void ResetIndicator();
        void Init();
        Transform GetTransform();
        void SetTarget(TargetableBase target);
        void SetPosition(Vector3 position);
        void SetSize(Vector3 size);
        void SetValid(bool valid);
        void SetActive(bool active);
    }
}