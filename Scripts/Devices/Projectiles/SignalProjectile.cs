using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SignalProjectile : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject sender;

    [Header("Settings")]
    [SerializeField] private float intensity;
    [SerializeField] private float lifespan;

    public static event EventHandler OnAnyProjectileImpact;
    public event EventHandler<OnProjectileImpactEventArgs> OnProjectileImpact;
    public event EventHandler OnProjectileLifespanEnd;

    public class OnProjectileImpactEventArgs : EventArgs
    {
        public ContactPoint contactPoint; 
    } 

    private void Update()
    {
        HandleLifespan();
    }

    public void SetProjectile(GameObject sender, float intensity)
    {
        this.sender = sender;
        this.intensity = intensity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnProjectileImpact?.Invoke(this, new OnProjectileImpactEventArgs { contactPoint = collision.contacts[0] });
        OnAnyProjectileImpact?.Invoke(this, EventArgs.Empty);

        SignalSender signalSender = collision.gameObject.GetComponentInParent<SignalSender>();

        if (signalSender != null)
        {
            Electrode component = collision.gameObject.GetComponent<Electrode>();
            if (component != null)
            {
                if (!component.Source) component.ReceiveSignal(intensity);
                else if (intensity > component.Power)
                {
                    component.SourcePower = intensity;
                    Electricity.Instance.UpdateElectrode(component);
                }
            }

            ElectrodeCollider electroCollider = collision.gameObject.GetComponent<ElectrodeCollider>();

            if(electroCollider != null )
            {
                if (!electroCollider.Electrode.Source) electroCollider.Electrode.ReceiveSignal(intensity);
                else if (intensity < component.Power)
                {
                    component.SourcePower = intensity;
                    Electricity.Instance.UpdateElectrode(component);
                }
            }
        }

        Destroy(gameObject);
    }

    private void HandleLifespan()
    {
        if (lifespan >= 0) lifespan -= Time.deltaTime;
        else
        {
            OnProjectileLifespanEnd?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }
}
