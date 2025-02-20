using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalExtensibleBridge : MonoBehaviour
{
    [Header("Identifiers")]
    [SerializeField] private int id;

    [Header("Electricity")]
    [SerializeField] private ElectricalDevice device;
    [SerializeField] private bool isPowered;

    [Header("Device Settings")]
    [SerializeField] private Transform bridgeTransform;
    [SerializeField] private Transform restPosition;
    [SerializeField] private Transform powerPosition;
    [SerializeField] private float moveSpeed;
    [SerializeField] private AnimationCurve moveCurve;

    [Header("State")]
    [SerializeField] private bool state;
    private GameObject player;
    private Coroutine doorMovement;

    private float notPoweredTimer;
    private const float NOT_POWERED_TIME_THRESHOLD = 0.1f;

    private bool previousPowered;

    public static event EventHandler<OnDoowPoweredEventArgs> OnDoorPower;
    public static event EventHandler<OnDoowPoweredEventArgs> OnDoorDePowered;

    public class OnDoowPoweredEventArgs : EventArgs
    {
        public int id;
    }

    private void OnEnable()
    {
        device.OnStateChange += CheckPower;
    }

    private void OnDisable()
    {
        device.OnStateChange -= CheckPower;
    }

    private void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        notPoweredTimer = 0f;
        previousPowered = isPowered;
    }

    void Update()
    {
        ManageState();

        HandlePowered();
    }

    private void ManageState()
    {
        if (state == isPowered) return;

        if (state) state = false;
        else
        {
            state = true;
        }


        TriggerMovement(state);
    }

    private void HandlePowered()
    {
        if (!isPowered)
        {
            notPoweredTimer += Time.deltaTime;

            if (notPoweredTimer >= NOT_POWERED_TIME_THRESHOLD && previousPowered)
            {
                OnDoorDePowered?.Invoke(this, new OnDoowPoweredEventArgs { id = id });
                previousPowered = false;
            }
        }
        else
        {
            if (!previousPowered)
            {
                OnDoorPower?.Invoke(this, new OnDoowPoweredEventArgs { id = id });
            }

            notPoweredTimer = 0;
            previousPowered = true;
        }
    }

    private void CheckPower(bool isPowered)
    {
        this.isPowered = isPowered;
    }

    private void TriggerMovement(bool state)
    {
        if (doorMovement != null) StopCoroutine(doorMovement);
        doorMovement = StartCoroutine(MoveToPosition(state ? powerPosition.position : restPosition.position));
    }

    private IEnumerator MoveToPosition(Vector3 position)
    {
        Vector3 startPosition = bridgeTransform.position;
        Vector3 endPosition = position;
        float distance = Vector3.Distance(startPosition, endPosition);
        float time = distance / moveSpeed;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            bridgeTransform.position = Vector3.Lerp(startPosition, endPosition, moveCurve.Evaluate(t / time));

            yield return null;
        }

        bridgeTransform.position = endPosition;
    }
}
