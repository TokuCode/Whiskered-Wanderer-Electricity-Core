using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalProjectileVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SignalProjectile signalProjectile;

    [Header("VisualEffects")]
    [SerializeField] private Transform trailVFXTransform;
    [SerializeField] private Transform impactVFXPrefab;

    [Header("Settings")]
    [SerializeField,Range(0f,1f)] private float trailDestroyTime;
    [SerializeField,Range(0f,2f)] private float impactDestroyTime;

    private void OnEnable()
    {
        signalProjectile.OnProjectileImpact += SignalProjectile_OnProjectileImpact;
        signalProjectile.OnProjectileLifespanEnd += SignalProjectile_OnProjectileLifespanEnd;
    }

    private void OnDisable()
    {
        signalProjectile.OnProjectileImpact -= SignalProjectile_OnProjectileImpact;
        signalProjectile.OnProjectileLifespanEnd -= SignalProjectile_OnProjectileLifespanEnd;
    }

    private void EndTrail()
    {
        trailVFXTransform.SetParent(null);
        Destroy(trailVFXTransform.gameObject, trailDestroyTime);
    }

    private void CreateImpactvfx(ContactPoint contactPoint)
    {
        Quaternion rotation = Quaternion.LookRotation(contactPoint.normal);

        Transform impactVFXPrefabTransform = Instantiate(impactVFXPrefab, transform.position, rotation);
        impactVFXPrefabTransform.SetParent(null);

        Destroy(impactVFXPrefabTransform.gameObject, impactDestroyTime);
    }

    private void SignalProjectile_OnProjectileImpact(object sender, SignalProjectile.OnProjectileImpactEventArgs e)
    {
        EndTrail();
        CreateImpactvfx(e.contactPoint);
    }

    private void SignalProjectile_OnProjectileLifespanEnd(object sender, System.EventArgs e)
    {
        EndTrail();
    }
}
