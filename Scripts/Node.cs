using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class Node
{
    [SerializeField] private Electrode electrode;
    private List<Node> contactNodes;
    private Circuit circuit;
    [SerializeField] private float weight;
    
    [SerializeField] private List<Electrode> contactElectrodes;

    public List<Node> ContactNodes {  get { return contactNodes; } }
    public Electrode Component { get { return electrode; } }
    public Circuit Circuit { get { return circuit; } set { circuit = value; } }
    public float Weight { get { return weight; } set { weight = value; } }

    public Node(Electrode electrode)
    {
        this.contactNodes = new List<Node>();
        this.electrode = electrode;
        this.contactElectrodes = new List<Electrode>();
        this.circuit = new Circuit();
        if(electrode.Source) this.weight = electrode.SourcePower;
        else this.weight = 0;
        this.circuit.AddNode(this);
    }

    public void LabelElectrodes()
    {
        contactElectrodes = new List<Electrode>();

        contactNodes.ForEach(node => contactElectrodes.Add(node.Component));
    }

    public void AddContact(Node contact)
    {
        if (contactNodes.Contains(contact))
        {
            return;
        }

        contactNodes.Add(contact);
        
        LabelElectrodes();
    }

    public void RemoveContact(Node contact)
    {
        if (contactNodes.Contains(contact))
        {
            contactNodes.Remove(contact);
        }

        LabelElectrodes();
    }

    public void ResetWeight()
    {
        this.weight = 0;
        if (Component.Source) this.weight = electrode.SourcePower;
    }

    public void Broadcast()
    {
        if (electrode.Power <= 0 || electrode.Device) return;

        contactNodes.ForEach(node => {
            if (weight > node.weight) node.electrode.ReceiveSignal(electrode.SendSignal(node.electrode));
            else if (electrode.DebugTool) Debug.Log($"{electrode.name} couldn't send signal to {node.electrode.name}");
        });
    }

    public void BroadcastSpecific(Node node)
    {
        if (electrode.Power <= 0 || electrode.Device) return;

        if (!contactNodes.Contains(node)) return;

        if(weight > node.weight) node.electrode.ReceiveSignal(electrode.SendSignal(node.electrode));
    }

    private Electrode GetComponent() => electrode;
}
