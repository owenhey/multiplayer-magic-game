using UnityEngine;

namespace Helpers {
    public class RotateAroundAxis : MonoBehaviour {
        public Vector3 Axis;
        public float speed;

        private void Update() {
            transform.Rotate(Axis, speed * Time.deltaTime, Space.Self);
        }
    }
}