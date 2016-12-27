using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour {

    private Color emissionColor;
    private float seed = 0;
	// Use this for initialization
	void Start () {
        emissionColor = gameObject.GetComponentInChildren<MeshRenderer>().material.GetColor("_EmissionColor");
        seed = Random.Range(0, 1000000);
    }
	
	// Update is called once per frame
	void Update () {
        // change emission color
        float emission = 2f + Mathf.Sin(seed + Time.time * 1.5f);
        Color em = emissionColor * Mathf.LinearToGammaSpace(emission);
        gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", em);
    }
}
