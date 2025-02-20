using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DrainerElectrodeEmissionVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Electrode electrode;
    [SerializeField] private Renderer _renderer;

    private Material material;

    private bool previouslyDraining;

    private void Awake()
    {
        material = _renderer.material;
        GeneralRenderingMethods.SetMaterialEmission(material, false);

        previouslyDraining = false;
    }

    private void LateUpdate()
    {
        HandleDraining();
    }

    private void HandleDraining()
    {
        if(previouslyDraining && !CircuitContainsSource())
        {
            GeneralRenderingMethods.SetMaterialEmission(material, false);
            previouslyDraining = false;
            return;
        }

        if(!previouslyDraining && CircuitContainsSource())
        {
            GeneralRenderingMethods.SetMaterialEmission(material, true);
            previouslyDraining = true;
        }
    }

    private bool CircuitContainsSource()
    {
       foreach(Node node in electrode.Node.Circuit.Elements)
       {
            if (node == electrode.Node) continue;
            if (node.Component.Type.Contains(Electrode.ComponentType.source)) return true;
       }

       return false;
    }
}
