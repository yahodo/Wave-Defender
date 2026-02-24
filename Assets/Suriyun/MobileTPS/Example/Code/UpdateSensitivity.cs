using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Suriyun.MobileTPS;

public class UpdateSensitivity : MonoBehaviour
{
    public GameCamera g_cam;
    public Text text;

    void Start()
    {
        g_cam = GameObject.FindObjectOfType<GameCamera>();
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = "Sensitivity " + (int)(g_cam.mouse_sensitivity*10) * 0.1f;
    }

}
