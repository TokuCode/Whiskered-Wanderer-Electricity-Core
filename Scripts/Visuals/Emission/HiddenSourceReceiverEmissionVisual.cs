using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenSourceReceiverEmissionVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private HiddenSourceReceiver hiddenSourceReceiver;
    [SerializeField] private Renderer _renderer;

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
        GeneralRenderingMethods.SetMaterialEmission(material, false);
    }

    private void HiddenSourceReceiver_OnHiddenSourceReceiverPower(object sender, System.EventArgs e)
    {
        GeneralRenderingMethods.SetMaterialEmission(material, true);

    }

    private void HiddenSourceReceiver_OnHiddenSourceReceiverDePower(object sender, System.EventArgs e)
    {
        GeneralRenderingMethods.SetMaterialEmission(material, false);
    }

}
