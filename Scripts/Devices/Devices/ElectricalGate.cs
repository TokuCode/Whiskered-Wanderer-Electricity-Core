using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalGate : MonoBehaviour
{
    [Header("Electrical Component")]
    [SerializeField] private Electrode electrode;

    [Header("Device Settings")]
    [SerializeField] private Transform gateTransform;
    [SerializeField] private Transform restPosition;
    [SerializeField] private Transform powerPosition;
    [SerializeField] private float moveSpeed;
    [SerializeField] private AnimationCurve moveCurve;

    [Header("Device Control")]
    [SerializeField] private bool state;
    private bool power => electrode.Power >= Electrode.ACTIVATION_THRESHOLD;
    private bool coherence => state == power;

    private void Update()
    {
        if (coherence) return;

        state = power;
        TriggerMovement();
    }

    private void TriggerMovement()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPosition(state ? powerPosition.position : restPosition.position));
    }

    private IEnumerator MoveToPosition(Vector3 position)
    {
        Vector3 startPosition = gateTransform.position;
        Vector3 endPosition = position;
        float distance = Vector3.Distance(startPosition, endPosition);
        float time = distance / moveSpeed;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            gateTransform.position = Vector3.Lerp(startPosition, endPosition, moveCurve.Evaluate(t / time));

            yield return null;
        }

        gateTransform.position = endPosition;
    }
}
