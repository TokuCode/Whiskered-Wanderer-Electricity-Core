using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrodeCollider : MonoBehaviour
{
    [Header("Electrical Component")]
    [SerializeField] private Electrode electrode;
    public Electrode Electrode {  get { return electrode; } }

    protected void OnCollisionEnter(Collision collision)
    {
        Electrode other = collision.collider.gameObject.GetComponent<Electrode>();

        if (other == null) return;

        electrode.AddContact(other);
    }

    protected void OnCollisionStay(Collision collision)
    {
        Electrode other = collision.collider.gameObject.GetComponent<Electrode>();

        if (other == null) return;

        electrode.AddContact(other);
    }

    protected void OnCollisionExit(Collision collision)
    {
        Electrode other = collision.collider.gameObject.GetComponent<Electrode>();

        if (other == null) return;

        electrode.RemoveContact(other);
    }
}
