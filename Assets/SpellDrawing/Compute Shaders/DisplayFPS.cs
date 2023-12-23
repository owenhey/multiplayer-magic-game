using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayFPS : MonoBehaviour
{
    [SerializeField] private bool active;
    [SerializeField] private GameObject textGameObject;
    
    
    private TextMeshProUGUI text;

    public void ShowFPS(bool b){
        active = b;
        OnSetActive(b);
    }

    private void Start() {
        text = textGameObject.GetComponent<TextMeshProUGUI>();
        OnSetActive(active);
    }

    private void Update() {
        textGameObject.SetActive(active);
    }

    private void OnSetActive(bool value){
        if(value){
            BeginDisplaying();
        }
        else{
            StopDisplaying();
        }
    }

    private void BeginDisplaying(){
        CancelInvoke();
        InvokeRepeating("UpdateFPS", 0, .25f);
    }

    private void StopDisplaying(){
        CancelInvoke();
    }

    private void UpdateFPS() {
        text.text = "FPS: " + (int)(1.0f / Time.smoothDeltaTime);
    }
}
