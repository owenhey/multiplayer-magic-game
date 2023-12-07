using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals {
    public interface IIndicator {
        void Init();
        Transform GetTransform();
        void SetPosition(Vector3 position);
        void SetSize(float size);
        void SetValid(bool valid);
        void SetActive(bool active);
    }
}