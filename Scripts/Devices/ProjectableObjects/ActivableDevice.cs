using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivableDevice : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ProjectableObjectActivation projectableObjectActivation;

    [Header("Electrical Component")]
    [SerializeField] private Electrode electrode;
    [SerializeField] private bool isActive;
    [SerializeField] private bool startActive;

    [Header("Device Control")]
    [SerializeField] protected bool state;

    public bool canBeActivated { get; protected set; }

    public bool Power => electrode.Power >= Electrode.ACTIVATION_THRESHOLD;
    private bool coherence => state == (Power && isActive);
    public bool IsActive => isActive;

    private void OnEnable()
    {
        projectableObjectActivation.OnProjectableObjectActivated += ProjectableObjectActivation_OnProjectableObjectActivated;
    }

    private void OnDisable()
    {
        projectableObjectActivation.OnProjectableObjectActivated -= ProjectableObjectActivation_OnProjectableObjectActivated;
    }

    protected virtual void Start()
    {
        isActive = startActive;
    }

    protected virtual void Update()
    {
        if (coherence) return;

        state = Power && isActive;
        ToggleActivation();
    }

    public void SetDeviceActive(bool state) => isActive = state;

    protected abstract void ToggleActivation();
    protected abstract void CheckCanBeActivated();

    private void ProjectableObjectActivation_OnProjectableObjectActivated(object sender, System.EventArgs e)
    {
        SetDeviceActive(!isActive);
    }
}
