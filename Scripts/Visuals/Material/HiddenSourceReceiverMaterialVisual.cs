using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenSourceReceiverMaterialVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private HiddenSourceReceiver hiddenSourceReceiver;
    [SerializeField] private Renderer _renderer;

    [Header("Settings")]
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;

    private Material material;

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
        material = _renderer.material;
        GeneralRenderingMethods.SetRendererMaterial(_renderer, offMaterial);
    }

    private void HiddenSourceReceiver_OnHiddenSourceReceiverPower(object sender, System.EventArgs e)
    {
        GeneralRenderingMethods.SetRendererMaterial(_renderer, onMaterial);
    }

    private void HiddenSourceReceiver_OnHiddenSourceReceiverDePower(object sender, System.EventArgs e)
    {
        GeneralRenderingMethods.SetRendererMaterial(_renderer, offMaterial);
    }

}
