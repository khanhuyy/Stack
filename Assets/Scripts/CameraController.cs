using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.GameOver += GameEvents_GameOver;
    }
    
    private void OnDisable()
    {
        GameEvents.GameOver -= GameEvents_GameOver;
    }

    private void GameEvents_GameOver()
    {
        if (gameObject.TryGetComponent(out Camera component))
        {
            StartCoroutine(SmoothZoomOutCo(component, 3.5f));
        }
    }

    private IEnumerator SmoothZoomOutCo(Camera cameraComponent, float target)
    {
        while (Mathf.Abs(cameraComponent.orthographicSize - target) > 0.01f)
        {
            cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, target, Time.deltaTime * 1);
            yield return null;
        }
        cameraComponent.orthographicSize = target; // Snap to the target size at the end
    }
}
