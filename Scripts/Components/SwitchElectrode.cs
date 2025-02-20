using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwitchElectrode : Electrode
{
    [Header("Switch Specific")]
    [SerializeField] private bool switchOn;
    public bool SwitchOn => switchOn;

    public event EventHandler<OnSwitchSetEventArgs> OnSwitchSet;

    public class OnSwitchSetEventArgs : EventArgs
    {
        public bool switchOn;
    }

    public override void ReceiveSignal(float intensity)
    {
        if (!switchOn) return;

        base.ReceiveSignal(intensity);
    }

    public void InitializeSwitch(bool switchOn)
    {
        this.switchOn = switchOn;

        if (Electricity.Instance)
        {
            Electricity.Instance.UpdateElectrode(this);
        }

        OnSwitchSet?.Invoke(this, new OnSwitchSetEventArgs { switchOn = switchOn });
    }


    public void SetSwitch(bool swichOn)
    {
        this.switchOn = swichOn;
        Electricity.Instance.UpdateElectrode(this);

        OnSwitchSet?.Invoke(this, new OnSwitchSetEventArgs { switchOn = switchOn });
    }
}
