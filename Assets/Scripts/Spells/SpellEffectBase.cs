using UnityEngine;

namespace Spells {
    public abstract class SpellEffectBase {
        protected SpellCastData _spellCastData;

        public void Init(SpellCastData castData) {
            _spellCastData = castData;
        }
    }
}