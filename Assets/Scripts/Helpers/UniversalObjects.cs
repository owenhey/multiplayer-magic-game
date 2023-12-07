using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Helpers{
    public class UniversalObjects : MonoBehaviour
    {
        public static UniversalObjects instance;

        public FPSDisplayer fpsDisplayer;
        public EventDisplayer eventDisplayer;

        private void Awake() {
            if(instance != null){
                Destroy(gameObject);
            }
            else{
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}