using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalElectrode : Electrode
{
    [Header("Physical Components")]
    protected Collider box;

    protected override void Start()
    {
        base.Start();
        box = GetComponent<Collider>();
    }

    protected void OnCollisionEnter(Collision collision)
    {
        Electrode other = collision.collider.gameObject.GetComponent<Electrode>();

        if (other == null) return;

        AddContact(other);
    }

    protected void OnCollisionStay(Collision collision)
    {
        Electrode other = collision.collider.gameObject.GetComponent<Electrode>();

        if (other == null) return;

        AddContact(other);
    }

    protected void OnCollisionExit(Collision collision)
    {
        Electrode other = collision.collider.gameObject.GetComponent<Electrode>();

        if (other == null) return;

        RemoveContact(other);
    }
}
