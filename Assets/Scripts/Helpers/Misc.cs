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
    }
}