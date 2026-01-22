using UnityEngine;

public class SunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimeController timeController;
    [SerializeField] private Light sunDirectionalLight;

    [Header("Daylight Times (24h)")]
    [SerializeField] private float sunriseHour = 6f;
    [SerializeField] private float sunsetHour = 18f;

    [Header("Sun Direction")]
    [SerializeField] private float sunriseAzimuth = 90f;
    [SerializeField] private float sunsetAzimuth = 270f;

    [Header("Sun Height")]
    [SerializeField] private float maxElevation = 60f;
    [SerializeField] private float belowHorizonElevation = -6f;

    [Header("Optional Smoothing")]
    [SerializeField] private AnimationCurve daylightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    

    private void LateUpdate()
    {
        if (!timeController || !sunDirectionalLight) return;

        float timeHours = timeController.CurrentTimeHours24;

        float t = Mathf.InverseLerp(sunriseHour, sunsetHour, timeHours);
        t = Mathf.Clamp01(t);
        t = Mathf.Clamp01(daylightCurve.Evaluate(t));

        float azimuth = Mathf.LerpAngle(sunriseAzimuth, sunsetAzimuth, t);

        float elevation01 = Mathf.Sin(t * Mathf.PI);
        float elevation = Mathf.Lerp(belowHorizonElevation, maxElevation, elevation01);

        Quaternion rot = Quaternion.Euler(-elevation, azimuth, 0f);
        Vector3 sunDirection = rot * Vector3.forward;

        sunDirectionalLight.transform.rotation = Quaternion.LookRotation(-sunDirection, Vector3.up);
    }
}