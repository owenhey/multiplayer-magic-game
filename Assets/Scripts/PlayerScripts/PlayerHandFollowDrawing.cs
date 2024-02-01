using System;
using System.Collections;
using System.Collections.Generic;
using Drawing;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

namespace PlayerScripts {
    public class PlayerHandFollowDrawing : LocalPlayerScript {
        [SerializeField] private float _rigWeightMax = .9f;
        [SerializeField] private float _rigWeightMin = .35f;
        [SerializeField] private float _rigAnimationTime = .1f;
        [SerializeField] private TrailRenderer _trail;
        
        [SerializeField] private float _targetPositionFactor = 1;
        [SerializeField] private Rig _rig;
        [SerializeField] private Transform _handTarget;

        private Vector3 _handStartPosition;
        private Tween _rigWeightTween;
        
        protected override void Awake() {
            base.Awake();
            _rig.weight = _rigWeightMin;
            _handStartPosition = _handTarget.localPosition;
            _trail.emitting = false;
        }

        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) return;
            DrawingManager.OnTranslatedStartDraw += OnStartDraw;
            DrawingManager.OnTranslatedDraw += OnDraw;
            DrawingManager.OnTranslatedEndDraw += OnEndDraw;
            DrawingManager.OnDrawCancelled += OnDrawCancelled;
        }

        private void OnDestroy() {
            DrawingManager.OnTranslatedStartDraw -= OnStartDraw;
            DrawingManager.OnTranslatedDraw -= OnDraw;
            DrawingManager.OnTranslatedEndDraw -= OnEndDraw;
            DrawingManager.OnDrawCancelled -= OnDrawCancelled;
        }

        private void OnStartDraw() {
            if (_rigWeightTween != null) {
                _rigWeightTween.Kill();
            }
            _trail.emitting = true;

            _rigWeightTween = DOTween.To(() => _rig.weight, x => _rig.weight = x, _rigWeightMax, _rigAnimationTime);
        }

        private void OnDraw(Vector2 point) {
            _handTarget.localPosition = _handStartPosition + _targetPositionFactor * new Vector3(point.x - .5f, point.y - .5f, 0);
        }

        private void OnEndDraw(Vector2[] _v, float _f) {
            OnDrawCancelled();
        }

        private void OnDrawCancelled() {
            if (_rigWeightTween != null) {
                _rigWeightTween.Kill();
            }
            _trail.emitting = false;

            _handTarget.DOKill();
            _handTarget.DOLocalMove(_handStartPosition, .1f);
            _rigWeightTween = DOTween.To(() => _rig.weight, x => _rig.weight = x, _rigWeightMin, _rigAnimationTime);
        }
    }
}