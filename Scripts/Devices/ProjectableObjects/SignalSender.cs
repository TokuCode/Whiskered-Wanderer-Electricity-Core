using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SignalSender : MonoBehaviour
{
    [Header("Electrical Component")]
    [SerializeField] private Electrode electrode;

    [Header("Device Settings")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float shootCooldown;
    [SerializeField] private bool parentProjectiles;

    private float shootTimer;
    public static event EventHandler OnAnyProjectileShot;
    public event EventHandler OnProjectileShot;

    private void OnEnable()
    {
        electrode.OnReceiveSignal += ShootSender;
    }

    private void OnDisable()
    {
        electrode.OnReceiveSignal -= ShootSender;
    }

    private void Start()
    {
        InitializeVariables();
    }

    private void Update()
    {
        HandleShootTimer();
    }
    private void InitializeVariables()
    {
        shootTimer = shootCooldown/2;
    }

    private void HandleShootTimer()
    {
        if (shootTimer > 0) shootTimer -= Time.deltaTime;
    }

    private void ShootSender()
    {
        if (shootTimer > 0) return;

        shootTimer = shootCooldown;

        float intensity = electrode.Power;

        if (intensity < Electrode.ACTIVATION_THRESHOLD) return;

        GameObject projectileGO = Instantiate(projectile, shootPosition.position, Quaternion.identity);

        SignalProjectile signalProjectile = projectileGO.GetComponent<SignalProjectile>();

        signalProjectile?.SetProjectile(gameObject, intensity);

        Rigidbody rbProjectile = projectileGO.GetComponent<Rigidbody>();

        rbProjectile?.AddForce(shootPosition.right.normalized * shootSpeed, ForceMode.VelocityChange);

        OnAnyProjectileShot?.Invoke(this, EventArgs.Empty);
        OnProjectileShot?.Invoke(this, EventArgs.Empty);

        if (parentProjectiles) projectileGO.transform.SetParent(transform);
    }
}
