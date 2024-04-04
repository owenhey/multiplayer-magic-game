using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace UI{
    public class HovererUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea(3, 10)] [SerializeField] string PrimaryText;
        [SerializeField] string SecondaryText;
        [SerializeField] string TertiaryText;
        [SerializeField] string FourthText;
        [SerializeField] string FifthText;
        [SerializeField] RectTransform HoverPoint;
        private bool _active = true;
        
        public bool Active{
            get{
                return _active;
            }
            set{
                _active = value;
                if(!_active && _hovering){
                    OnPointerExit(null);
                }
            }
        }

        private bool _hovering = false;

        private Canvas __canvas;
        private Canvas _canvas{
            get{
                if(__canvas == null){
                    __canvas = GetComponentInParent<Canvas>();
                }
                return __canvas;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Hello");
            if(!_active) return;

            _hovering = true;
            Vector3 worldCenterPosition = HoverPoint.TransformPoint(HoverPoint.rect.center);
            // Convert world position to the canvas's local space
            Vector2 positionRelativeToCanvas = _canvas.transform.InverseTransformPoint(worldCenterPosition);

            HoverManager.Show(PrimaryText, SecondaryText, TertiaryText, FourthText, FifthText, positionRelativeToCanvas);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Hello2");
            if(!_active) return;
            
            _hovering = false;
            HoverManager.Hide();
        }

        private void OnDisable(){
            if(_hovering && Active){
                OnPointerExit(null);
            }
        }

        private void Reset() {
            if (transform.childCount == 0) {
                var newChild = new GameObject();
                newChild.transform.parent = transform;
                HoverPoint = newChild.GetComponent<RectTransform>();
            }
        }
        
        public void SetText(string[] texts) {
            switch (texts.Length) {
                case 1:
                    SetText(texts[0], "", "", "", "");
                    break;
                case 2:
                    SetText(texts[0],texts[1], "", "", "");
                    break;
                case 3:
                    SetText(texts[0],texts[1],texts[2], "", "");
                    break;
                case 4:
                    SetText(texts[0],texts[1],texts[2],texts[3], "");
                    break;
                case 5:
                    SetText(texts[0],texts[1],texts[2],texts[3],texts[4]);
                    break;
            }
        }

        public void SetText(string primarytext) => SetText(new string[] { primarytext });

        public void SetText(string primaryText, string secondaryText, string teriaryText, string fourthText, string fifthText){
            PrimaryText = primaryText;
            SecondaryText = secondaryText;
            TertiaryText = teriaryText;
            FourthText = fourthText;
            FifthText = fifthText;
        }
    }
}