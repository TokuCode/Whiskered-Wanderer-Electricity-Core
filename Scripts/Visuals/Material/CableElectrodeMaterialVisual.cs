using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableElectrodeMaterial : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Electrode electrode;
    [SerializeField] private Renderer _renderer;

    [Header("Settings")]
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;

    private bool IsPowered => electrode.Power >= Electrode.ACTIVATION_THRESHOLD;

    private const float NOT_POWERED_TIME_THRESHOLD = 0.25f;
    private float notPoweredTimer;
    private bool previousPowered;

    private Material material;

    private void Awake()
    {
        material = _renderer.material;
        GeneralRenderingMethods.SetRendererMaterial(_renderer, offMaterial);
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
                GeneralRenderingMethods.SetRendererMaterial(_renderer, offMaterial);
                previousPowered = false;
            }
        }
        else
        {
            if (!previousPowered)
            {
                GeneralRenderingMethods.SetRendererMaterial(_renderer, onMaterial);
            }

            notPoweredTimer = 0;
            previousPowered = true;
        }
    }
}
