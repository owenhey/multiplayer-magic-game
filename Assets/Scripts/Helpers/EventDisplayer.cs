using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers{
    public enum EventDisplayTypes{
        Information,
        Warning,
        Green,
        Blue,
        Yellow
    }

    [System.Serializable]
    public struct Color2{
        public Color color1;
        public Color color2;
    }

    public class EventDisplayer : MonoBehaviour
    {
        [Header("Stats")]
        public int maxEvents = 5;

        [Header("Colors")]
        public Color2 information;
        public Color2 warning;
        public Color2 blue;
        public Color2 green;
        public Color2 yellow;

        [Header("Child References")]
        [SerializeField] private Transform eventsDisplayAreaTransform;
        [SerializeField] private GameObject eventDisplayAreaPrefab;

        private LinkedList<EventDisplayerArea> activeEventAreas = new LinkedList<EventDisplayerArea>();

        private void Start() {
            // DisplayEvent(
            //     "Hello",
            //     "How are you doing?",
            //     EventDisplayTypes.Information
            // );
            // DisplayEvent(
            //     "Go away",
            //     "I am going to eat your soul and then kill your children slowly",
            //     EventDisplayTypes.Warning
            // );
            // DisplayEvent(
            //     "My name is Kimiko",
            //     "I really hope the ending of this season isn't complete shit",
            //     EventDisplayTypes.Blue
            // );
        }

        public void DisplayEvent(string eventTitle, string eventDescription, EventDisplayTypes eventType){
            Color titleColor = Color.white;
            Color descriptionColor = Color.white;

            switch(eventType){
                case EventDisplayTypes.Information:
                    titleColor = information.color1;
                    descriptionColor = information.color2;
                    break;
                case EventDisplayTypes.Warning:
                    titleColor = warning.color1;
                    descriptionColor = warning.color2;
                    break;
                case EventDisplayTypes.Green:
                    titleColor = green.color1;
                    descriptionColor = green.color2;
                    break;
                case EventDisplayTypes.Blue:
                    titleColor = blue.color1;
                    descriptionColor = blue.color2;
                    break;
                case EventDisplayTypes.Yellow:
                    titleColor = yellow.color1;
                    descriptionColor = yellow.color2;
                    break;
            }
            var obj = Instantiate(eventDisplayAreaPrefab, eventsDisplayAreaTransform);
            var eda = obj.GetComponent<EventDisplayerArea>();
            activeEventAreas.AddFirst(eda);
            eda.Setup(eventTitle, eventDescription, titleColor, descriptionColor);

            PushEventAreas();
        }

        private void PushEventAreas(){
            while(activeEventAreas.Count > maxEvents + 1){
                activeEventAreas.RemoveLast();
            }
            foreach (var item in activeEventAreas)
            {
                if(item != null){
                    item?.Push(maxEvents);
                }
            }
        }
    }
}