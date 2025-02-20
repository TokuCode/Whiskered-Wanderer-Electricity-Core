using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalDevice : MonoBehaviour
{
    public event Action<bool> OnStateChange;
    
    public Electrode electrode;
    
    private bool isPowered => electrode.Power >= Electrode.ACTIVATION_THRESHOLD;

    private bool state;
    
    void Update()
    {
        bool coherence = isPowered == state;

        if (coherence) return;

        state = isPowered;
        OnStateChange?.Invoke(state);
    }
}
