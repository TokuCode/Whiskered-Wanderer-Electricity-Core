using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElectricalExtensibleBridgeOld : MonoBehaviour
{
    [Header("Identifiers")]
    [SerializeField] private int id;

    [Header("Electrical Component")]
    [SerializeField] private Electrode electrode;

    [Header("Device Settings")]
    [SerializeField] private Transform model;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private float extension;
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;
    [SerializeField] private float scalingTime;
    [SerializeField] private AnimationCurve scalingCurve;

    [Header("Device Control")]
    [SerializeField] private bool state;
    private bool IsPowered => electrode.Power >= Electrode.ACTIVATION_THRESHOLD;

    private const float NOT_POWERED_TIME_THRESHOLD = 0.2f;
    private float notPoweredTimer;
    private bool previousPowered;
    private bool coherence => state == IsPowered;

    private Vector3 minPos;
    private Vector3 maxPos;

    public static event EventHandler<OnExtensibleBridgePowerEventArgs> OnExtensibleBridgePower;
    public static event EventHandler<OnExtensibleBridgePowerEventArgs> OnExtensibleBridgeDePower;

    public class OnExtensibleBridgePowerEventArgs : EventArgs
    {
        public int id;
    }

    private void Start()
    {
        minScale = model.localScale;
        maxScale = new Vector3(model.localScale.x, model.localScale.y, model.localScale.z + extension);

        minPos = model.localPosition;
        maxPos = new Vector3(model.localPosition.x, model.localPosition.y, model.localPosition.z + extension/2);
    }

    private void Update()
    {
        HandlePowered();

        if (coherence) return;

        state = IsPowered;

        if (!IsPowered)
        {
            Contract();
        } else
        {
            Extend();
        }
    }

    private void HandlePowered()
    {
        if (!IsPowered)
        {
            notPoweredTimer += Time.deltaTime;

            if (notPoweredTimer >= NOT_POWERED_TIME_THRESHOLD && previousPowered)
            {
                OnExtensibleBridgeDePower?.Invoke(this, new OnExtensibleBridgePowerEventArgs { id = id });
                previousPowered = false;
            }
        }
        else
        {
            if (!previousPowered)
            {
                OnExtensibleBridgePower?.Invoke(this, new OnExtensibleBridgePowerEventArgs { id = id });
            }

            notPoweredTimer = 0;
            previousPowered = true;
        }
    }

    private void Contract()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleObstacle(minScale, minPos));
    }

    private void Extend()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleObstacle(maxScale, maxPos));
    }

    private IEnumerator ScaleObstacle(Vector3 scale, Vector3 position)
    {
        Vector3 startScale = model.localScale;
        Vector3 endScale = scale;

        Vector3 startPos = model.localPosition;
        Vector3 endPos = position;

        float time = scalingTime;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            model.localScale = Vector3.Lerp(startScale, endScale, scalingCurve.Evaluate(t / time));
            model.localPosition = Vector3.Lerp(startPos, endPos, scalingCurve.Evaluate(t / time));

            boxCollider.size = Vector3.Lerp(startScale, endScale, scalingCurve.Evaluate(t / time));
            boxCollider.center = Vector3.Lerp(startPos, endPos, scalingCurve.Evaluate(t / time));

            yield return null;
        }

        model.localScale = endScale;
        model.localPosition = position;
    }
}
