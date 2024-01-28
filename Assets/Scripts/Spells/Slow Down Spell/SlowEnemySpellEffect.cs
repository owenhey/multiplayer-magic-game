using Core;
using PlayerScripts;
using Visuals;

namespace Spells {
    [SpellEffect("Slow Enemy")]
    public class SlowEnemySpellEffect : SingleCastSpellEffect{
        public override void BeginSpell() {
            if (TargetManager.GetTargetable(_spellCastData.TargetData.TargetId) is not PlayerTargetable) return;
            
            var targetPlayer = (TargetManager.GetTargetable(_spellCastData.TargetData.TargetId) as PlayerTargetable).Player;
            float amount = _spellCastData.SpellDefinition.GetAttributeValue("speed_factor");
            float duration = _spellCastData.SpellDefinition.GetAttributeValue("duration");
            targetPlayer.PlayerReferences.PlayerStatus.ClientAddStatus(new PlayerStatusEffect(
                "slow_down",
                PlayerStatusType.SpeedMultiplier,
                amount,
                duration
            ));
        }
    }
}