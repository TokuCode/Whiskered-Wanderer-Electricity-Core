using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenderProjectileShotVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SignalSender signalSender;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform shootVFXPrefab;

    [Header("Settings")]
    [SerializeField,Range(1f,2.5f)] private float shootVFXDestroyTime;

    private void OnEnable()
    {
        signalSender.OnProjectileShot += SignalSender_OnProjectileShot;
    }

    private void OnDisable()
    {
        signalSender.OnProjectileShot -= SignalSender_OnProjectileShot;
    }

    private void CreateShootVFX()
    {
        Transform shootVFXPrefabTransform = Instantiate(shootVFXPrefab, shootPoint.position, shootPoint.rotation);
        Destroy(shootVFXPrefabTransform.gameObject, shootVFXDestroyTime);
    }

    private void SignalSender_OnProjectileShot(object sender, System.EventArgs e)
    {
        CreateShootVFX();
    }
}
