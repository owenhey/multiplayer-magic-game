using FishNet.Connection;
using Helpers;
using PlayerScripts;
using UnityEngine;

namespace Spells {
    /// <summary>
    /// SpellCastData contains all information about a spell being cast, passed to the SpellEffect so that it can do its magic
    /// </summary>
    public class SpellCastData {
        public SpellTargetData TargetData;
        public NetworkConnection CastingPlayer;
        public int SpellId;
        public SpellDefinition SpellDefinition => SpellIder.Instance.GetSpell(SpellId);
        public float Damage;
        public float Duration;
    }
    
    /// <summary>
    /// Created by the indicator UI when you try to cast something
    /// </summary>
    public class SpellTargetData {
        public bool Cancelled;
        public Vector3 TargetPosition;
        public NetworkConnection TargetPlayer;
    }
}