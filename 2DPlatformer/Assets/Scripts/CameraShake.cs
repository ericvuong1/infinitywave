using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraShake : MonoBehaviour {

    public Camera mainCam;

    float shakeAmount = 0;


    private void Awake()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShakeCamera(0.1f, 0.2f);
        }
    }

    public void ShakeCamera(float amt, float duration)
    {
        shakeAmount = amt;
        InvokeRepeating("DoShake", 0f, 0.01f);
        Invoke("StopShake", duration);
    }


    void DoShake()
    {
        if (shakeAmount > 0)
        {
            Vector3 camPos = mainCam.transform.position;

            float OffsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float OffsetY = Random.value * shakeAmount * 2 - shakeAmount;
            camPos.x += OffsetX;
            camPos.y += OffsetY;

            mainCam.transform.position = camPos;
        }
    }


    void StopShake()
    {
        CancelInvoke("DoShake");
        mainCam.transform.localPosition = Vector3.zero;
    }
}
