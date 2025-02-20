using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchElectrodeLightVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SwitchElectrode switchElectrode;
    [SerializeField] private List<Light> lights;

    [Header("Settings")]
    [SerializeField] private float onIntensity = 1f;
    [SerializeField] private float offIntensity = 0f;

    private bool IsPowered => switchElectrode.Power >= Electrode.ACTIVATION_THRESHOLD;

    private const float NOT_POWERED_TIME_THRESHOLD = 0.1f;
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
                GeneralRenderingMethods.SetLightsIntensity(lights, onIntensity);
            }
            else
            {
                GeneralRenderingMethods.SetLightsIntensity(lights, offIntensity);

            }
        }
        else
        {
            GeneralRenderingMethods.SetLightsIntensity(lights, offIntensity);
        }
    }


    private void SwitchElectrode_OnSwitchSet(object sender, SwitchElectrode.OnSwitchSetEventArgs e)
    {
        HandleSwitchStateChange();
    }
}
