using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CableVFXHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Electrode electrode;
    [SerializeField] private VisualEffect cableVFX;

    [Header("Settings")]
    [SerializeField] private bool enableVFX = true;

    [Header("Cable On Settings")]
    [SerializeField, ColorUsage(true, true)] private Color onColor;
    [SerializeField, Range (0f,300f)] private float onParticleRatio;
    [SerializeField, Range(0f,0.15f)] private float onParticleScale;
    [SerializeField, Range(0f,0.1f)] private float onDispersion;

    [Header("Cable Off Settings")]
    [SerializeField, ColorUsage(true, true)] private Color offColor;
    [SerializeField, Range(0f, 300f)] private float offParticleRatio;
    [SerializeField, Range(0f, 0.15f)] private float offParticleScale;
    [SerializeField, Range(0f, 0.1f)] private float offDispersion;

    private bool IsPowered => electrode.Power >= Electrode.ACTIVATION_THRESHOLD;

    private const string COLOR_PROPERTY = "ParticleColor";
    private const string PARTICLE_RATIO_PROPERTY = "ParticleRatio";
    private const string PARTICLE_SCALE_PROPERTY = "ParticleScale";
    private const string DISPERSION_PROPERTY = "Dispersion";

    private const float NOT_POWERED_TIME_THRESHOLD = 0.5f;
    private float notPoweredTimer;
    private bool previousPowered;

    private void Awake()
    {
        TurnVFX(false); //TurnVFX(true)
        //SetVFXOn(false);
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
                //SetVFXOn(false);
                TurnVFX(false);
                previousPowered = false;
            }
        }
        else
        {
            if (!previousPowered)
            {
                //SetVFXOn(true);
                TurnVFX(true);
            }

            notPoweredTimer = 0;
            previousPowered = true;
        }
    }

    private void TurnVFX(bool on)
    {
        if (on && enableVFX)
        {
            cableVFX.Play();
        }
        else
        {
            cableVFX.Stop();
        }
    }

    private void SetVFXColor(Color color)
    {
        if (!cableVFX.HasVector4(COLOR_PROPERTY))
        {
            Debug.Log("PropertyNotFound");
            return;
        }

        cableVFX.SetVector4(COLOR_PROPERTY, color);
    }

    private void SetVFXParticleRatio(float particleRatio)
    {
        if (!cableVFX.HasFloat(PARTICLE_RATIO_PROPERTY))
        {
            Debug.Log("PropertyNotFound");
            return;
        }

        cableVFX.SetFloat(PARTICLE_RATIO_PROPERTY, particleRatio);
    }

    private void SetVFXParticleScale(float particleScale)
    {
        if (!cableVFX.HasFloat(PARTICLE_SCALE_PROPERTY))
        {
            Debug.Log("PropertyNotFound");
            return;
        }

        cableVFX.SetFloat(PARTICLE_SCALE_PROPERTY, particleScale);
    }

    private void SetVFXDispersion(float dispersion)
    {
        if (!cableVFX.HasFloat(DISPERSION_PROPERTY))
        {
            Debug.Log("PropertyNotFound");
            return;
        }

        cableVFX.SetFloat(DISPERSION_PROPERTY, dispersion);
    }

    private void SetVFXOn(bool cableOn)
    {
        if (cableOn)
        {
            SetVFXColor(onColor);
            SetVFXParticleRatio(onParticleRatio);
            SetVFXParticleScale(onParticleScale);
            SetVFXDispersion(onDispersion);
        }
        else
        {
            SetVFXColor(offColor);
            SetVFXParticleRatio(offParticleRatio);
            SetVFXParticleScale(offParticleScale);
            SetVFXDispersion(offDispersion);
        }
    }
}
