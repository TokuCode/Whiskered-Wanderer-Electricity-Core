using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposableElectrode : MonoBehaviour
{
    [SerializeField] private Electrode electrode;
    void Start()
    {
        Electricity.Instance.AddComponentToList(electrode);
    }

    private void OnDestroy()
    {
        Electricity.Instance.RemoveComponentFromList(electrode);
    }
}
