using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers {
    public class LookAtCamera : LookAtTarget {
        private void Start() {
            Target = Camera.main.transform;
        }
    }
}