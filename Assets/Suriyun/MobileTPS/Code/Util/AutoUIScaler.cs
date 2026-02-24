using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoUIScaler : MonoBehaviour {
    public Camera cam;
    public AnimationCurve scale;

    float aspect_ratio = 1.5f;

	void Update () {
        aspect_ratio = cam.pixelWidth / (float)cam.pixelHeight;
        this.transform.localScale = Vector3.one * scale.Evaluate(aspect_ratio);
        this.transform.position = cam.ScreenToWorldPoint(Vector3.zero);
        //Debug.Log(aspect_ratio);
	}
}
