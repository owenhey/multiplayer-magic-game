using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private readonly static int maxLineLength = 20;
        
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

        private bool brokeUpLines = false;

        private void Start() {
            BreakUpLines();
        }

        private void BreakUpLines() {
            if (brokeUpLines) return;
            brokeUpLines = true;
            
            PrimaryText = BreakUpString(PrimaryText);
            SecondaryText = BreakUpString(SecondaryText);
            TertiaryText = BreakUpString(TertiaryText);
            FourthText = BreakUpString(FourthText);
            FifthText = BreakUpString(FifthText);
        }

        private string BreakUpString(string input) {
            if (string.IsNullOrEmpty(input))
                return input;

            var lines = new List<string>();
            string[] tokens = input.Split(' ');
            int currentIndex = 0;
            StringBuilder currentLine = new();
            while (currentIndex < tokens.Length) {
                currentLine.Append(tokens[currentIndex]);
                currentIndex++;
                if (currentLine.Length > maxLineLength) {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                    continue;
                }
                currentLine.Append(' ');
            }
            lines.Add(currentLine.ToString());

            foreach (var v in lines) {
                Debug.Log($"line: {v}");
            }
            return string.Join("<br>", lines);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!_active) return;

            _hovering = true;
            Vector3 worldCenterPosition = HoverPoint.TransformPoint(HoverPoint.rect.center);
            // Convert world position to the canvas's local space
            Vector2 positionRelativeToCanvas = _canvas.transform.InverseTransformPoint(worldCenterPosition);

            HoverManager.Show(PrimaryText, SecondaryText, TertiaryText, FourthText, FifthText, positionRelativeToCanvas);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
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

        public void SetText(string primaryText, string secondaryText, string teriaryText, string fourthText, string fifthText) {
            brokeUpLines = false;
            
            PrimaryText = primaryText;
            SecondaryText = secondaryText;
            TertiaryText = teriaryText;
            FourthText = fourthText;
            FifthText = fifthText;
            
            BreakUpLines();
        }
    }
}