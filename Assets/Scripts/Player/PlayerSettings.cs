using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class PlayerSettings : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            CursorSettings();
            FPS();
        }

        void FPS() {
            Application.targetFrameRate = 144;
        }

        void CursorSettings() {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}