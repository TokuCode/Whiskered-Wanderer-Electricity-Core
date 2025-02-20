using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrainerElectrodeLightVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Electrode electrode;
    [SerializeField] private List<Light> lights;

    [Header("Settings")]
    [SerializeField] private float onIntensity = 1f;
    [SerializeField] private float offIntensity = 0f;

    private bool previouslyDraining;

    private void Awake()
    {
        GeneralRenderingMethods.SetLightsIntensity(lights, offIntensity);
        previouslyDraining = false;
    }

    private void LateUpdate()
    {
        HandleDraining();
    }

    private void HandleDraining()
    {
        if (previouslyDraining && !CircuitContainsSource())
        {
            GeneralRenderingMethods.SetLightsIntensity(lights, offIntensity);
            previouslyDraining = false;
            return;
        }

        if (!previouslyDraining && CircuitContainsSource())
        {
            GeneralRenderingMethods.SetLightsIntensity(lights, onIntensity);
            previouslyDraining = true;
        }
    }

    private bool CircuitContainsSource()
    {
        foreach (Node node in electrode.Node.Circuit.Elements)
        {
            if (node == electrode.Node) continue;
            if (node.Component.Type.Contains(Electrode.ComponentType.source)) return true;
        }

        return false;
    }
}
