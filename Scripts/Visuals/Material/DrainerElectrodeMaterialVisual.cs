using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrainerElectrodeMaterialVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Electrode electrode;
    [SerializeField] private Renderer _renderer;

    [Header("Settings")]
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;

    private Material material;

    private bool previouslyDraining;

    private void Awake()
    {
        material = _renderer.material;
        GeneralRenderingMethods.SetRendererMaterial(_renderer, offMaterial);

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
            GeneralRenderingMethods.SetRendererMaterial(_renderer, offMaterial);

            previouslyDraining = false;
            return;
        }

        if (!previouslyDraining && CircuitContainsSource())
        {
            GeneralRenderingMethods.SetRendererMaterial(_renderer, onMaterial);

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
