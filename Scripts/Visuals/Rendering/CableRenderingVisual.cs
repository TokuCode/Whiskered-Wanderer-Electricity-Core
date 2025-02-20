using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableRenderingVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform[] corners;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Settings")]
    [SerializeField] private bool enableLineRenderer = true;
    [SerializeField] private float thickness;
    [SerializeField] private bool updateLineRenderer;

    private void Start()
    {
        DrawLineRenderer();
    }

    private void LateUpdate()
    {
        UpdateLineRenderer();
    }

    private void DrawLineRenderer()
    {
        if (!enableLineRenderer) return;

        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;
        lineRenderer.positionCount = corners.Length;

        for (int i = 0; i < corners.Length; i++)
        {
            lineRenderer.SetPosition(i, corners[i].position);
        }
    }

    private void UpdateLineRenderer()
    {
        if (!updateLineRenderer) return;

        DrawLineRenderer();
    }
}
