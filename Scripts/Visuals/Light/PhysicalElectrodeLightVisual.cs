using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalElectrodeLightVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Electrode electrode;
    [SerializeField] private List<Light> lights;

    [Header("Settings")]
    [SerializeField] private float onIntensity = 1f;
    [SerializeField] private float offIntensity = 0f;

    private bool IsPowered => electrode.Power >= Electrode.ACTIVATION_THRESHOLD;

    private const float NOT_POWERED_TIME_THRESHOLD = 0.25f;
    private float notPoweredTimer;
    private bool previousPowered;

    private void Awake()
    {
        GeneralRenderingMethods.SetLightsIntensity(lights,offIntensity);
    }

    private void LateUpdate()
    {
        HandlePowered();
    }

    private void HandlePowered()
    {
        if (!IsPowered)
        {
            notPoweredTimer += Time.deltaTime;

            if (notPoweredTimer >= NOT_POWERED_TIME_THRESHOLD && previousPowered)
            {
                GeneralRenderingMethods.SetLightsIntensity(lights, offIntensity);
                previousPowered = false;
            }
        }
        else
        {
            if (!previousPowered)
            {
                GeneralRenderingMethods.SetLightsIntensity(lights, onIntensity);
            }

            notPoweredTimer = 0;
            previousPowered = true;
        }
    }
}
