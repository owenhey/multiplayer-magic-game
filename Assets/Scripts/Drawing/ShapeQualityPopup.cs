using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ShapeQualityPopup : MonoBehaviour {
    public TextMeshProUGUI text;
   
    public CanvasGroup cg;
    public float animTime = .15f;

    public Color amazingColor;
    public Color goodColor;
    public Color decentColor;
    public Color badColor;

    public void Setup(float score) {
        transform.DOScale(Vector3.one * 1.5f, animTime * .5f).OnComplete(() => {
            transform.DOScale(Vector3.one, animTime * .5f).OnComplete(() => {
                cg.DOFade(0, 1.0f).OnComplete(() => Destroy(gameObject));
            });
        });
        
        if (score > .9f) {
            text.text = "AMAZING!";
            text.color = amazingColor;
        }
        else if (score > .75f) {
            text.text = "Great!";
            text.color = goodColor;
        }
        else if (score > .5f) {
            text.text = "Okay";
            text.color = decentColor;
        }
        else {
            text.text = "Failed";
            text.color = badColor;
        }
    }
}
