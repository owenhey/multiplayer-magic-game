

using System.Collections.Generic;
using Core;
using Core.TeamScripts;
using UnityEngine;

namespace Visuals{
    public class ObjectTargetable : TargetableBase {
        [SerializeField] private Material _matToCopy;
        [SerializeField] private MeshRenderer _mr;
        private Material _mat;
        private Color _baseColor;
        
        private void Awake() {
            _mat = new Material(_matToCopy);
            _baseColor = _mat.color;

            List<Material> newMats = new();
            for (int i = 0; i < _mr.sharedMaterials.Length; i++) {
                if (_mr.sharedMaterials[i] == _matToCopy) {
                    newMats.Add(_mat);
                }
                else {
                    newMats.Add(_mr.sharedMaterials[i]);
                }
            }
            
            _mr.SetMaterials(newMats);
        }
        
        public override bool IsValidTarget(Teams clientTeam, TargetTypes targetTypes) {
            return targetTypes.HasFlag(TargetTypes.Objects);
        }

        public override void SetSelected(bool selected) {
            _mat.color = selected ? Color.red : _baseColor;
        }
    }
}