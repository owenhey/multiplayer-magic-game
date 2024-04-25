using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Visuals;

public class LightningTest : MonoBehaviour
{
    [SerializeField] private SingleLightningBehavior _lightningPrefab;
    
    [ContextMenu("Strike")]
    public void Strike() {
        StartCoroutine(StrikeC());
    }

    private IEnumerator StrikeC() {
        Instantiate(_lightningPrefab).InitSmall(transform.position, 10f);
        yield return new WaitForSeconds(1.0f);
        Instantiate(_lightningPrefab).InitBang(transform.position, 2.0f);
    }
}
