using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DrainerDevice : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Electrode electrode;

    private bool previouslyDraining;

    public static event EventHandler OnDrainerStartDraining;
    public static event EventHandler OnDrainerStopDraining;

    private void Awake()
    {
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
            OnDrainerStopDraining?.Invoke(this, EventArgs.Empty);
            previouslyDraining = false;
            return;
        }

        if (!previouslyDraining && CircuitContainsSource())
        {
            OnDrainerStartDraining?.Invoke(this, EventArgs.Empty);
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
