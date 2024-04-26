using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Visuals;

public class LightningTest : MonoBehaviour
{
    [SerializeField] private SingleLightningBehavior _lightningPrefab;
    [SerializeField] private VisualEffect _vfx;

    [ContextMenu("Strike")]
    public void Strike() {
        StartCoroutine(StrikeC());
    }

    private IEnumerator StrikeC() {
        Instantiate(_lightningPrefab).InitSmall(transform.position, 1.0f);
        yield return new WaitForSeconds(.8f);
        Instantiate(_lightningPrefab).InitBang(transform.position, .4f);
        yield return new WaitForSeconds(.1f);

        var o = Instantiate(_vfx, transform.position, transform.rotation);
        o.transform.parent = null;
        o.gameObject.SetActive(true);
        Destroy(o, 2);
        o.Play();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Strike();
        }
    }
}
