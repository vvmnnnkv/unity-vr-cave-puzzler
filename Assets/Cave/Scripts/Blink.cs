using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    // base emission values
    private float baseEmission = 2f;

    private float baseBlinkVariation = 1f;

    // peak values. originally same as base
    private float peakEmission = 2f;

    private float peakBlinkVariation = 1f;

    private Color emissionColor;
    private float seed = 0;

    private float emission;
    private float blinkVariation;
    private float t = 0;

    // how fast emission will animate from peak to base values
    private float restoreSpeed = .4f;

    // Use this for initialization
    void Start()
    {
        // get starting emission color
        emissionColor = gameObject.GetComponentInChildren<MeshRenderer>().material.GetColor("_EmissionColor");
        seed = Random.Range(0, 1000000);
    }

    // Update is called once per frame
    void Update()
    {
        // emission and variation values will change from peak to base smoothly
        emission = Mathf.Lerp(peakEmission, baseEmission, t);
        blinkVariation = Mathf.Lerp(peakBlinkVariation, baseBlinkVariation, t);
        if (t >= 1)
        {
            t = 1;
        }
        else
        {
            t = t + Time.deltaTime * restoreSpeed;
        }
        // calc new emission color
        float em = emission + blinkVariation * Mathf.Sin(seed + Time.time * 1.5f);
        Color ec = emissionColor * Mathf.LinearToGammaSpace(em);
        // change emission color
        gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", ec);
    }

    public void Shine()
    {
        // peak values for highlighting crystal
        peakEmission = 15f;
        peakBlinkVariation = 2f;
        t = 0;
    }

    public void Darken()
    {
        // lowest values for darkening crystal
        peakEmission = -4f;
        peakBlinkVariation = 1f;
        t = 0;
    }
}