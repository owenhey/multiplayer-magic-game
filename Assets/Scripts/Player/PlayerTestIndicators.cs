using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Visuals;

namespace PlayerScripts{
    public class PlayerTestIndicators : LocalPlayerScript
    {
        [SerializeField] private PlayerReferences _refs;

        private bool _isLocal;

        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) {
                this.enabled = false;
            }

            _isLocal = isOwner;
        }

        private void Update() {
            if (!_isLocal) return;

            if (Input.GetKey(KeyCode.Mouse0)) {
                var indicator = IndicatorManager.Instance.GetIndicator(IndicatorTypes.Sphere);
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = _refs.Cam.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    indicator.SetActive(true);
                    indicator.SetPosition(hit.point);
                    indicator.SetValid(hit.collider.gameObject.layer == 0);
                }
                else {
                    indicator.SetActive(false);
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0)) {
                IndicatorManager.Instance.GetIndicator(IndicatorTypes.Sphere).SetActive(false);
                Vector3 target = IndicatorManager.Instance.GetIndicator(IndicatorTypes.Sphere).GetTransform().position;
                StartCoroutine(Teleport(target));
            }
        }

        private IEnumerator Teleport(Vector3 target) {
            _refs.PlayerModel.AnimateTwirl(true);
            yield return new WaitForSeconds(.35f);
            _refs.PlayerMovement.Warp(target);
            yield return new WaitForSeconds(.2f);
            _refs.PlayerModel.AnimateTwirl(false);
        }
    }
}