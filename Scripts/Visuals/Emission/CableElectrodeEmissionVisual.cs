using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableElectrodeEmissionVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Electrode electrode;
    [SerializeField] private Renderer _renderer;
    private bool IsPowered => electrode.Power >= Electrode.ACTIVATION_THRESHOLD;

    private const float NOT_POWERED_TIME_THRESHOLD = 0.25f;
    private float notPoweredTimer;
    private bool previousPowered;

    private Material material;

    private void Awake()
    {
        material = _renderer.material;
        GeneralRenderingMethods.SetMaterialEmission(material, false);
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
                GeneralRenderingMethods.SetMaterialEmission(material, false);
                previousPowered = false;
            }
        }
        else
        {
            if (!previousPowered)
            {
                GeneralRenderingMethods.SetMaterialEmission(material, true);
            }

            notPoweredTimer = 0;
            previousPowered = true;
        }
    }
}
