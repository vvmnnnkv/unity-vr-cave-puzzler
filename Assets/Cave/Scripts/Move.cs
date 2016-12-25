using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    private Vector3 startPos;
    private Vector3 endPos;
    public GameObject obj;
    private bool triggered = false;
    private float speed = 0.1f;
    private float progress = 0f;

    // Use this for initialization
    void Start () {
        endPos = obj.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (triggered)
        {
            float distance = Vector3.Distance(Camera.main.transform.position, endPos);
            if (distance > 0.05f)
            {
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, endPos, speed);
            }
            else
            {
                Camera.main.transform.position = endPos;
                triggered = false;
            }
        }
	}

    public void MoveCamera()
    {
        startPos = Camera.main.transform.position;
        triggered = true;
    }
}
