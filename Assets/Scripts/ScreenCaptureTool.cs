using System;
using UnityEngine;

public class ScreenCaptureTool : MonoBehaviour
{
    [SerializeField] string filename;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ScreenCapture.CaptureScreenshot($"{filename}_{DateTime.Now.Ticks}.png");
        }
    }
}