using DG.Tweening;
using UnityEngine;

namespace UI {
    public class LineUI : MonoBehaviour {
        [SerializeField] private RectTransform _rt;

        public void Setup(Vector2 start, Vector2 end, float width = 20, float animateTime = 0) {
            _rt.anchoredPosition = start;
            
            // Figure out rotation
            Vector2 direction = (end - start).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle -= 90;
            _rt.localRotation = Quaternion.Euler(0, 0, angle);

            _rt.DOSizeDelta(new Vector2(width, (end-start).magnitude), .3f).SetEase(Ease.OutQuad);
        }
    }
}