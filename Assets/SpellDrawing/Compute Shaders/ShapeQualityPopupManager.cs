using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Helpers;

public class ShapeQualityPopupManager : MonoBehaviour
{
    public static ShapeQualityPopupManager Instance;

    public RectTransform spawnParent;
    public ShapeQualityPopup popupPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;    
    }

    public void ShowPopup(DrawingResults r, float score) {
        
        float timePercent = Mathf.Max(0, Mathf.Abs(r.TotalTime - r.Drawing.TimeTarget) / r.Drawing.TimeTarget);

        float distanceAccuracy = Misc.RemapClamp(r.AverageDistance, .02f, 0.1f, 1, 0);
        
        float totalDistanceAcc = Misc.RemapClamp(r.PercentDistanceError, .03f, .1f, 1, 0);

        var obj = Instantiate(popupPrefab, spawnParent);
        
        obj.Setup(distanceAccuracy);
        return;
    }
}
