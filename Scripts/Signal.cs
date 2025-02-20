using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Signal : MonoBehaviour 
{
    [SerializeField] private float intensity;
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private float duration;
    [SerializeField] private float timer;

    private void Update()
    {
        HandleSignalTimer();
    }

    private void HandleSignalTimer()
    {
        if (timer < duration) timer += Time.deltaTime;
        else timer = 0f;
    }

    public float GetPower()
    {
        timer = Mathf.Clamp(timer, 0f, duration);

        float power = intensity * powerCurve.Evaluate(timer);

        if (power <= Electrode.ACTIVATION_THRESHOLD)
        {
            Expire();
            return 0;
        }

        return power;
    }

    public float GetConstant() => intensity;


    public Signal SetSignal(float intensity, AnimationCurve powerCurve, float duration)
    {
        this.intensity = intensity;
        this.powerCurve = powerCurve;
        this.duration = duration;
        this.timer = 0f;
    
        return this;
    }

    public void Expire()
    {
        if (gameObject == null) return;

        Electrode electrode = gameObject.GetComponent<Electrode>();
        
        if(electrode != null)
        {
            if(electrode.Signals.Contains(this)) electrode.Signals.Remove(this);
        }

        Destroy(this);
    }
}
