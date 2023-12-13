using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Camera Information
    public Transform cameraTransform;
    private Vector3 orignalCameraPos;

    // Shake Parameters
    public float shakeDuration = 2f;
    public float shakeAmount = 0.7f;

    private bool canShake = false;
    private float _shakeTimer;

    bool waiting;

    // Start is called before the first frame update
    void Start()
    {
        orignalCameraPos = cameraTransform.localPosition;
    }
    IEnumerator waitingFalse()
    {
        yield return new WaitForSeconds(2f);
        waiting = false;
    }
    // Update is called once per frame
    void Update()
    {

        if (canShake)
        {
            StartCameraShakeEffect();
        }

    }
    public void ShakeCamera()
    {
        if (!waiting)
        {
            orignalCameraPos = cameraTransform.localPosition;
            waiting = true;
            StartCoroutine(waitingFalse());
            canShake = true;
            _shakeTimer = shakeDuration;
        }
    }

    public void StartCameraShakeEffect()
    {
        if (_shakeTimer > 0)
        {
            cameraTransform.localPosition = orignalCameraPos + Random.insideUnitSphere * shakeAmount;
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            _shakeTimer = 0f;
            cameraTransform.localPosition = orignalCameraPos;
            canShake = false;
        }
    }

}
