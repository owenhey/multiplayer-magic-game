using UnityEngine;

namespace Spells {
    public abstract class SpellEffectBase {
        private static int __keyCounter = 0;
        protected static int _keyCounter {
            get {
                return ++__keyCounter;
            }
        }
        protected SpellCastData _spellCastData;

        public void Init(SpellCastData castData) {
            _spellCastData = castData;
        }
    }
}