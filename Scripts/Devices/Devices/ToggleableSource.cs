using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleableSource : MonoBehaviour
{
    [Header("Identifiers")]
    [SerializeField] private int id;

    [Header("Components")]
    [SerializeField] private Transform source;
    [SerializeField] private Transform stunnedSource;
    [Space]
    [SerializeField] private Transform powerPosition;
    [SerializeField] private Transform restPosition;

    [Header("Settings")]
    [SerializeField] private bool isOn;
    [SerializeField] private bool startOn;

    public int ID => id;

    private void Start()
    {
        InitializeSource();
    }

    private void InitializeSource()
    {
        if (startOn) TurnOnSource();
        else TurnOffSource();
    }

    public void TurnOnSource()
    {
        source.position = powerPosition.position;
        stunnedSource.position = restPosition.position;

        isOn = true;
    }

    public void TurnOffSource()
    {
        source.position = restPosition.position;
        stunnedSource.position = powerPosition.position;

        isOn = true;
    }

    public void ToggleSource()
    {
        if (isOn) TurnOffSource();
        else TurnOnSource();
    }
}
