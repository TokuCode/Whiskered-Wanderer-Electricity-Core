using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenSourceReceiverLightVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private HiddenSourceReceiver hiddenSourceReceiver;
    [SerializeField] private List<Light> lights;

    [Header("Settings")]
    [SerializeField] private float onIntensity = 1f;
    [SerializeField] private float offIntensity = 0f;

    private void OnEnable()
    {
        hiddenSourceReceiver.OnHiddenSourceReceiverPower += HiddenSourceReceiver_OnHiddenSourceReceiverPower;
        hiddenSourceReceiver.OnHiddenSourceReceiverDePower += HiddenSourceReceiver_OnHiddenSourceReceiverDePower;
    }

    private void OnDisable()
    {
        hiddenSourceReceiver.OnHiddenSourceReceiverPower -= HiddenSourceReceiver_OnHiddenSourceReceiverPower;
        hiddenSourceReceiver.OnHiddenSourceReceiverDePower -= HiddenSourceReceiver_OnHiddenSourceReceiverDePower;
    }

    private void Awake()
    {
        GeneralRenderingMethods.SetLightsIntensity(lights, offIntensity);
    }

    private void HiddenSourceReceiver_OnHiddenSourceReceiverPower(object sender, System.EventArgs e)
    {
        GeneralRenderingMethods.SetLightsIntensity(lights, onIntensity);
    }

    private void HiddenSourceReceiver_OnHiddenSourceReceiverDePower(object sender, System.EventArgs e)
    {
        GeneralRenderingMethods.SetLightsIntensity(lights, offIntensity);
    }
}
