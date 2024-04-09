using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers {
    public static class Misc {
        public static float Remap(float value, float baseLow, float baseHigh, float endLow, float endHigh) {
            return endLow + (value - baseLow) * (endHigh - endLow) / (baseHigh - baseLow);
        }


        public static float RemapClamp(float value, float baseLow, float baseHigh, float endLow, float endHigh) {
            value = Mathf.Max(baseLow, Mathf.Min(baseHigh, value));
            return endLow + (value - baseLow) * (endHigh - endLow) / (baseHigh - baseLow);
        }

        public static Vector3 GetRandomOffsetFromScore(float effectiveness, float maxRadius, bool includeY) {
            float randomnessRadius = Remap(effectiveness, 0, 1, maxRadius, 0);
            if (includeY) {
                Vector3 offset = Random.insideUnitSphere * randomnessRadius;
                return offset;
            }
            else {
                Vector2 offset = Random.insideUnitCircle * randomnessRadius;
                return new Vector3(offset.x, 0, offset.y);
            }
            
        }
        
        /// <summary>
        /// Also searches the input GameObject for this component
        /// </summary>
        public static T FindComponentInParent<T>(this GameObject gameObject) where T : Component{
            Transform currentTransform = gameObject.transform;
            while (currentTransform != null){
                T component = currentTransform.GetComponent<T>();
                if (component != null)
                    return component;

                currentTransform = currentTransform.parent;
            }

            return null;
        }

        public static RectTransform RT(this GameObject gameObject) {
            return gameObject.GetComponent<RectTransform>();
        }

        public static RectTransform RT(this Transform transform) {
            return transform.gameObject.RT();
        }
        
        public static void ForEachChild(this Transform transform, System.Action<Transform> action){
            if (transform == null || action == null) return;
            for (int i = transform.childCount - 1; i >= 0; i--){
                action(transform.GetChild(i));
            }
        }
    }
}