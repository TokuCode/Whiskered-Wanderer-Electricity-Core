using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HiddenSourceReceiver : MonoBehaviour
{
    [Header("Identifiers")]
    [SerializeField] private int id;

    [Header("Components")]
    [SerializeField] private Transform energyzedPos;
    [SerializeField] private Transform deEnergyzedPos;
    [SerializeField] private Transform hiddenSource;

    [Header("Settings")]
    [SerializeField] private float timeEnergyzed;

    private bool isEnergyzed;
    private float energyzedTimer;

    public static event EventHandler<OnHiddenSourcePowerEventArgs> OnAnyHiddenSourceReceiverPower;
    public static event EventHandler<OnHiddenSourcePowerEventArgs> OnAnyHiddenSourceReceiverDePower;

    public event EventHandler OnHiddenSourceReceiverPower;
    public event EventHandler OnHiddenSourceReceiverDePower;

    public class OnHiddenSourcePowerEventArgs : EventArgs
    {
        public int id;
    }

    private void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        energyzedTimer = 0;
        MoveHiddenSource(deEnergyzedPos);
        isEnergyzed = false;
    }

    private void Update()
    {
        HandleEnergyzation();
    }

    private void HandleEnergyzation()
    {
        if (energyzedTimer > 0) energyzedTimer -= Time.deltaTime;

        if(energyzedTimer <= 0)
        {
            DeEnergyzeReceiver();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.TryGetComponent(out SignalProjectile signalProjectile))
        {
            EnergyzeReceiver();
        }
    }

    private void EnergyzeReceiver()
    {
        MaxTimer();

        if (!isEnergyzed)
        {
            MoveHiddenSource(energyzedPos);
            isEnergyzed = true;
            OnAnyHiddenSourceReceiverPower?.Invoke(this, new OnHiddenSourcePowerEventArgs { id = id });
            OnHiddenSourceReceiverPower?.Invoke(this, EventArgs.Empty);
            
        }
    }

    private void DeEnergyzeReceiver()
    {
        ResetTimer();

        if (isEnergyzed)
        {
            MoveHiddenSource(deEnergyzedPos);
            isEnergyzed = false;
            OnAnyHiddenSourceReceiverDePower?.Invoke(this, new OnHiddenSourcePowerEventArgs { id = id });
            OnHiddenSourceReceiverDePower?.Invoke(this, EventArgs.Empty);
        }
    }

    private void MoveHiddenSource(Transform transform)
    {
        hiddenSource.transform.position = transform.position;
    }

    private void ResetTimer() => energyzedTimer = 0f;
    private void MaxTimer() => energyzedTimer = timeEnergyzed;
}
