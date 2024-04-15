using Core;
using Core.Damage;
using PlayerScripts;
using Visuals;

namespace Spells {
    [SpellEffect("Slow Enemy")]
    public class SlowEnemySpellEffect : SingleCastSpellEffect{
        public override void BeginSpell() {
            var statusable = TargetManager.GetTargetable(_spellCastData.TargetData.TargetId).Damagable.Statusable;
            float amount = _spellCastData.SpellDefinition.GetAttributeValue("speed_factor");
            float duration = _spellCastData.SpellDefinition.GetAttributeValue("duration");
            statusable.ClientAddStatus(new StatusEffect(
                "slow_down",
                StatusType.SpeedMultiplier,
                amount,
                duration
            ));
        }
    }
}