using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SourceVFXHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private VisualEffect sourceVFX;

    [Header("Settings")]
    [SerializeField] private bool enableVFX = true;

    private void Awake()
    {
        TurnVFX(false);
    }

    private void Start()
    {
        TurnVFX(true);
    }

    private void TurnVFX(bool on)
    {
        if (on && enableVFX)
        {
            sourceVFX.Play();
        }
        else
        {
            sourceVFX.Stop();
        }
    }
}
