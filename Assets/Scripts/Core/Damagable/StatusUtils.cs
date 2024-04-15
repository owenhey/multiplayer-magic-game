using System;
using UnityEngine;

namespace Core.Damage {
    public enum StatusType {
        Stunned,
        SpeedMultiplier,
    }
    
    public struct StatusesData {
        public bool Stunned;
        public float SpeedMultiplier;
    }

    [System.Serializable]
    public class StatusEffect {
        public string Key;
        public StatusType Type;
        public float Amount;
        public float Duration;
        
        public StatusEffect(){}
        /// <summary>
        /// Creates a status effect
        /// </summary>
        /// <param name="key"> Unique key in case you need to remove this </param>
        /// <param name="type"> Type of status </param>
        /// <param name="amount"> Amount of status</param>
        /// <param name="duration"> Set to negative one for infinite </param>
        public StatusEffect(string key, StatusType type, float amount, float duration) {
            Key = key;
            Type = type;
            Amount = amount;
            Duration = duration;
        }

        public (string, Color) GetPopupData() {
            switch (Type) {
                case StatusType.Stunned:
                    return ("Stunned!", Color.red);
                case StatusType.SpeedMultiplier:
                    if (Amount >= 1) {
                        return ("Hastened!", Color.green);
                    }
                    else {
                        return ("Slowed!", Color.cyan);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}