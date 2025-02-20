using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Electricity : MonoBehaviour
{
    [SerializeField] private List<Electrode> electrodes;

    public List<Electrode> Electrodes { get { return electrodes; } }

    [SerializeField] private List<Circuit> electricalCircuits;

    public List<Circuit> Circuits { get { return electricalCircuits; } }

    public static Electricity Instance { get; private set; }

    private List<Circuit> rechargeNeededCircuits;

    private float powerTimer;

    private void Awake()
    {
        Instance = this;
        electricalCircuits = new List<Circuit>();
        rechargeNeededCircuits = new List<Circuit>();
    }

    private void Start()
    {
        SetComponentsList();
    }

    private void Update()
    {
        if (powerTimer < Electrode.ELECTRICITY_TICK_DURATION) powerTimer += Time.deltaTime;
        else
        {
            powerTimer = 0f;
            PowerCircuits();
        }
    }

    private void PowerCircuits()
    {
        foreach (Circuit circuit in Circuits)
        {
            circuit.PowerCircuit();
        }
    }

    private void LateUpdate()
    {
        if (rechargeNeededCircuits.Count <= 0) return;

        rechargeNeededCircuits.ForEach(circuit => circuit.ResolveCircuit());

        rechargeNeededCircuits.Clear();
    }

    private void SetComponentsList()
    {
        electrodes = new List<Electrode>(Resources.FindObjectsOfTypeAll<Electrode>());
    }

    public void AddComponentToList(Electrode component)
    {
        if (electrodes.Contains(component)) return;

        electrodes.Add(component);
    }

    public void RemoveComponentFromList(Electrode component)
    {
        if (!electrodes.Contains(component)) return;

        List<Electrode> contacts = new List<Electrode>(component.RetrieveContacts());

        contacts.ForEach(contact =>
        {
            component.RemoveContact(contact);
        });

        component.Node.Circuit.Dispose();

        electrodes.Remove(component);
    }

    public void ConnectComponents(Electrode a, Electrode b)
    {
        Circuit circuitA = a.Node.Circuit;
        Circuit circuitB = b.Node.Circuit;

        if (circuitA == circuitB)
        {
            return;
        }

        if (circuitA == null && circuitB == null)
        {
            Circuit circuit = new Circuit();

            circuit.AddNode(a.Node);
            circuit.AddNode(b.Node);

            rechargeNeededCircuits.Add(circuit);
        }
        else if (circuitA != null && circuitB == null)
        {
            circuitA.AddNode(b.Node);

            rechargeNeededCircuits.Add(circuitA);
        }
        else if (circuitA == null && circuitB != null)
        {
            circuitB.AddNode(a.Node);

            rechargeNeededCircuits.Add(circuitB);
        }
        else
        {
            Circuit circuit = Circuit.MergeCircuit(circuitA, circuitB);
            rechargeNeededCircuits.Add(circuit);
        }

        SortCircuits();
    }

    public void DisconnectComponents(Circuit circuit)
    {
        List<Circuit> reorganizedCircuits = ReorganizeCircuit(circuit);

        reorganizedCircuits.ForEach(c =>
        {
            rechargeNeededCircuits.Add(c);
        });

        SortCircuits();
    }

    public void SortCircuits()
    {
        Circuits.Sort((a, b) => a.Elements.Count - b.Elements.Count);
    }

    private List<Circuit> ReorganizeCircuit(Circuit circuit)
    {
        List<Circuit> circuits = new List<Circuit>();

        List<Electrode> componentsToPlaceRemaining = new List<Electrode>();

        circuit.Elements.ForEach(e => componentsToPlaceRemaining.Add(e.Component));
        circuit.Dispose();

        do
        {
            Electrode component = componentsToPlaceRemaining.FirstOrDefault();

            if (component == null) return circuits;

            Circuit circuitTemp = new Circuit();

            RecursiveFillCircuit(ref circuitTemp, component, ref componentsToPlaceRemaining);

            circuits.Add(circuitTemp);

        } while (componentsToPlaceRemaining.Count > 0);

        return circuits;
    }

    private void RecursiveFillCircuit(ref Circuit circuit, Electrode component, ref List<Electrode> toFillFrom)
    {
        if (toFillFrom == null || !toFillFrom.Contains(component)) return;

        toFillFrom.Remove(component);
        circuit.AddNode(component.Node);

        foreach (Electrode contact in component.RetrieveContacts())
        {
            if (!toFillFrom.Contains(contact)) continue;

            RecursiveFillCircuit(ref circuit, contact, ref toFillFrom);
        }
    }

    public void UpdateElectrode(Electrode component)
    {
        //component.Node.Circuit.UpdateNodeForward(component.Node);
        component.Node.Circuit.ResolveCircuit();
    }

    public void FlushCircuitTasks(Circuit circuit)
    {
        circuit.cancellationTokens.ForEach(token => token.Cancel());
    }
}
