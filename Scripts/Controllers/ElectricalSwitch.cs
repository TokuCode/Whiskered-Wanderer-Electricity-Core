using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalSwitch : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SwitchElectrode switchElectrode;

    [Header("Identifiers")]
    [SerializeField] private int id;

    public int ID => id;
    public SwitchElectrode SwitchElectrode => switchElectrode;
    public bool IsOn => switchElectrode.SwitchOn;

    private bool hasBeenInitialized = false;
    private bool shouldInitialize;

    private bool initialState;

    private void Update()
    {
        InitializeOnFirstFrame();
    }

    private void InitializeOnFirstFrame()
    {
        if (!shouldInitialize) return;
        if (hasBeenInitialized) return;

        switchElectrode.InitializeSwitch(initialState);
        hasBeenInitialized = true;

    }

    public void InitializeSwitch(bool isOn)
    {
        shouldInitialize = true;
        initialState = isOn;
    }

    public void SetIsOn(bool isOn) => switchElectrode.SetSwitch(isOn);
}
