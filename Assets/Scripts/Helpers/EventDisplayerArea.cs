using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

namespace Helpers {
    public class EventDisplayerArea : MonoBehaviour {
        [Header("Animation Stats")] [SerializeField]
        private float timeToDie = 5;

        [SerializeField] private float animationTime = .25f;

        [Header("References")] [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private int slot;
        private RectTransform rectTransform;

        public void Setup(string _titleText, string _descriptionText, Color titleColor, Color descriptionColor) {
            slot = -1;
            rectTransform = GetComponent<RectTransform>();

            titleText.text = _titleText;
            descriptionText.text = _descriptionText;

            titleText.color = titleColor;
            descriptionText.color = descriptionColor;

            MoveToSlot(slot, true);
            Invoke("FadeAway", timeToDie);
            Invoke("DestroyAfterTime", timeToDie);
        }

        public void Push(int slotToDestroySelf) {
            slot++;

            MoveToSlot(slot);

            if (slot == slotToDestroySelf) {
                DestroyAfterTime();
            }
        }

        private void DestroyAfterTime() {
            CancelInvoke();
            Destroy(gameObject, animationTime);
        }

        private void FadeAway() {
            canvasGroup.DOFade(0, animationTime);
        }

        private void MoveToSlot(int slot, bool instant = false) {
            Vector2 newPos = GetAnchoredPosOfSlot(slot);
            if (instant) {
                rectTransform.anchoredPosition = newPos;
            }
            else {
                rectTransform.DOAnchorPos(newPos, animationTime);
            }
        }

        private Vector2 GetAnchoredPosOfSlot(int slot) {
            return new Vector2(0, slot * 100);
        }

        private void OnDestroy() {
            canvasGroup.DOKill();
            rectTransform.DOKill();
        }
    }
}