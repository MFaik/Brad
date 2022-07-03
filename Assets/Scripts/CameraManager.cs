using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Vector3 _startPosition;

    [HideInInspector] public float Intensity;
    [HideInInspector] public float ShakeTimer;

    void Start() {
        _startPosition = transform.position;
    }

    void Update() {
        if(ShakeTimer > 0){
            ShakeTimer -= Time.deltaTime;
            transform.position = _startPosition + (Vector3)Random.insideUnitCircle * Intensity;
        } else 
            transform.position = _startPosition;
    }

    static CameraManager s_currentCamera;

    public static void ShakeCamera(float intensity, float duration) {
        if(s_currentCamera == null)
            s_currentCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>();
        s_currentCamera.Intensity = intensity;
        s_currentCamera.ShakeTimer = duration;
    }
}