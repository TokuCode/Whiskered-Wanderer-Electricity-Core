using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableElectrode : Electrode
{
    [Header("Cable Specifics")]
    [SerializeField] private Transform[] corners;
    [SerializeField] private float radius;
    [Space]
    [SerializeField] private bool enableDistanceCheckOverride;

    private GameObject player;

    private const string PLAYER_TAG = "Player";
    private const float PLAYER_DISTANCE_TO_UPDATE = 30f;
    private const int FRAME_UPDATE_INTERVAL = 3;

    private int frameNumber;

    protected override void Start()
    {
        base.Start();
        InitializeVariables();
        ResetFrameNumber();
    }

    private void FixedUpdate()
    {
        FrameUpdateLogic();
    }

    private void InitializeVariables()
    {
        player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
    }

    private void FrameUpdateLogic()
    {
        HandleFrameUpdateCount();

        if (ShouldUpdateByFrame()) 
        {
            ResetFrameNumber();
            FindNearElectricalComponents();
        }
    }

    private void FindNearElectricalComponents()
    {
        if (!CheckPlayerClose() && !enableDistanceCheckOverride) return;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 direction = (corners[i + 1].position - corners[i].position).normalized;
            float distance = Vector3.Distance(corners[i].position, corners[i + 1].position);

            RaycastHit[] hits = Physics.SphereCastAll(corners[i].position, radius, direction, distance);

            List<Electrode> electrodesDetected = new List<Electrode>();

            foreach (RaycastHit hit in hits)
            {
                Electrode other = hit.collider.GetComponent<Electrode>();

                ElectrodeCollider otherCol = hit.collider.GetComponent<ElectrodeCollider>();

                if (other == null && otherCol == null) continue;

                if(other != null)
                {
                    electrodesDetected.Add(other);

                    AddContact(other);

                    continue;
                }

                electrodesDetected.Add(otherCol.Electrode);

                AddContact(otherCol.Electrode);
            }

            List<Electrode> ToRemove = new List<Electrode>();

            foreach (Electrode contact in contacts)
            {
                if (!electrodesDetected.Contains(contact)) ToRemove.Add(contact);
            }

            ToRemove.ForEach(contact => RemoveContact(contact));
        }
    }

    //public override bool CheckContact(Electrode other)
    //{
    //    if (!other.Source && !other.Sender) return false;
    //
    //    return base.CheckContact(other);
    //}

    private bool CheckPlayerClose()
    {
        if (!player) return true;
        if (Vector3.Distance(transform.position, player.transform.position) <= PLAYER_DISTANCE_TO_UPDATE) return true;

        return false;
    }

    private void HandleFrameUpdateCount() => frameNumber++;
    private void ResetFrameNumber() => frameNumber = 0;
    private bool ShouldUpdateByFrame() => frameNumber >= FRAME_UPDATE_INTERVAL;
}
