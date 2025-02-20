using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchElectrodeMaterialVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SwitchElectrode switchElectrode;
    [SerializeField] private Renderer _renderer;

    [Header("Materials")]
    [SerializeField] private Material energizedOnMaterial;
    [SerializeField] private Material deEnergyzedOnMaterial;
    [Space]
    [SerializeField] private Material energyzedOffMaterial;
    [SerializeField] private Material deEnergyzedOffMaterial;

    private bool IsPowered => switchElectrode.Power >= Electrode.ACTIVATION_THRESHOLD;

    private const float NOT_POWERED_TIME_THRESHOLD = 0.25f;
    private float notPoweredTimer;
    private bool previousPowered;

    private bool switchPowered;


    private void OnEnable()
    {
        switchElectrode.OnSwitchSet += SwitchElectrode_OnSwitchSet;
    }

    private void OnDisable()
    {
        switchElectrode.OnSwitchSet -= SwitchElectrode_OnSwitchSet;
    }

    private void Awake()
    {
        switchPowered = false;
        HandleSwitchStateChange();
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
                switchPowered = false;
                previousPowered = false;

                HandleSwitchStateChange();
            }
        }
        else
        {
            if (!previousPowered)
            {
                switchPowered = true;

                HandleSwitchStateChange();
            }

            notPoweredTimer = 0;
            previousPowered = true;
        }

    }

    private void HandleSwitchStateChange()
    {
        if (switchElectrode.SwitchOn)
        {
            if (switchPowered)
            {
                GeneralRenderingMethods.SetRendererMaterial(_renderer, energizedOnMaterial);
            }
            else
            {
                GeneralRenderingMethods.SetRendererMaterial(_renderer, deEnergyzedOnMaterial);
            }
        }
        else
        {
            if (switchPowered)
            {
                GeneralRenderingMethods.SetRendererMaterial(_renderer, energyzedOffMaterial);
            }
            else
            {
                GeneralRenderingMethods.SetRendererMaterial(_renderer, deEnergyzedOffMaterial);
            }
        }
    }


    private void SwitchElectrode_OnSwitchSet(object sender, SwitchElectrode.OnSwitchSetEventArgs e)
    {
        HandleSwitchStateChange();
    }
}
