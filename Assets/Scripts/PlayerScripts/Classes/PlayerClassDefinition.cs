using UnityEngine;

namespace PlayerScripts.Classes {
    [CreateAssetMenu(fileName = "PlayerClass", menuName = "ScriptableObjects/PlayerClass", order = 0)]
    public class PlayerClassDefinition : ScriptableObject {
        public PlayerClass PlayerClass;
        public Color ClassColor;
        // Some other stuff, I'm sure
    }
}