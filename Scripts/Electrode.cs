using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Electrode : MonoBehaviour
{
    public delegate void ReceiveSignalDel();
    public event ReceiveSignalDel OnReceiveSignal; 

    protected const float MAX_POWER = 10000;
    public const float ACTIVATION_THRESHOLD = 20f;
    public const float ELECTRICITY_TICK_DURATION = 1f;

    [Header("Debug Tools")]
    [SerializeField] protected bool debug;
    public bool DebugTool => debug;

    [Header("Electricity Component")]

    [SerializeField] protected float sourcePower;
    [SerializeField] protected float power;
    public float Power => power;
    public float SourcePower { get { return sourcePower; } set { sourcePower = value; } }

    [Header("Transmission Variables")]
    [SerializeField] protected List<Signal> signals;
    [SerializeField] protected AnimationCurve powerCurve;

    public List<Signal> Signals { get { return signals; } }

    public enum ComponentType
    {
        source,
        sender,
        device
    }

    [SerializeField] private ComponentType[] type;

    private bool source;
    public bool Source { get { return source; } }
    private bool sender;
    public bool Sender { get { return sender; } }
    private bool device;
    public bool Device { get { return device; } }
    public ComponentType[] Type => type;

    [Header("Circuit Representation")]
    [SerializeField] protected Node node;
    public Node Node { get { return node; } set { node = value; } }

    [SerializeField] protected List<Electrode> contacts;

    protected virtual void Start()
    {
        signals = new List<Signal>();
        contacts = new List<Electrode>();
        SetElectrodeType();
        ResetPower();
        node = new Node(this);
    }

    protected virtual void Update()
    {
        CalculatePower();
    }

    protected void SetElectrodeType()
    {
        List<ComponentType> types = type.ToList();

        if (type.Contains(ComponentType.source) && !type.Contains(ComponentType.device)) source = true;

        if (type.Contains(ComponentType.sender)) sender = true;

        if (type.Contains(ComponentType.device) && !type.Contains(ComponentType.source)) device = true;
    }

    public void AddContact(Electrode other, bool enableContact)
    {
        if (!CheckContact(other)) return;

        if (contacts.Contains(other)) return;

        contacts.Add(other);

        node.AddContact(other.node);

        if (enableContact)
        {
            other.AddContact(this, false);
            return;
        }
        
        if (!Node.Circuit.Elements.Contains(other.Node)) Electricity.Instance.ConnectComponents(this, other);
    }

    public void AddContact(Electrode other)
    {
        AddContact(other, true);
    }

    public void RemoveContact(Electrode other, bool enableContact)
    {
        if (!contacts.Contains(other)) return;

        contacts.Remove(other);

        node.RemoveContact(other.node);

        if (enableContact)
        {
            other.RemoveContact(this, false);
            return;
        }

        Electricity.Instance.DisconnectComponents(Node.Circuit);
    }

    public void RemoveContact(Electrode other)
    {
        RemoveContact(other, true);
    }

    public virtual bool CheckContact(Electrode other)
    {
        if (!Electricity.Instance.Electrodes.Contains(other)) return false;

        if (!this.sender && !other.sender) return false;

        return true;
    }

    public virtual void ReceiveSignal(float intensity)
    {
        if (intensity <= 0) return;

        Signal signal = gameObject.AddComponent<Signal>();
        signal.SetSignal(intensity, powerCurve, ELECTRICITY_TICK_DURATION);
        signals.Add(signal);

        CalculatePower();

        if (OnReceiveSignal != null) OnReceiveSignal();
    }

    public virtual float SendSignal(Electrode otherComponent)
    {
        float distance = Vector3.Distance(transform.position, otherComponent.transform.position);

        float intensity = Mathf.Max(power, 0f);

        if (debug) Debug.Log($"{name} sending signal of {intensity} to {otherComponent.name}");

        return intensity;
    }

    private void CalculatePower()
    {
        power = 0f;

        if (signals == null) return;

        if(signals.Count <= 0) return;

        List<Signal> signalsCopy = new List<Signal>(signals);

        signalsCopy.ForEach(signal => power += signal.GetPower());
    }

    public void SetPower(float power)
    {
        if (!source) return;

        this.power = power;

        Electricity.Instance.UpdateElectrode(this);
    }

    public virtual void ResetPower()
    {
        List<Signal> signalsCopy = new List<Signal>(signals);

        signalsCopy.ForEach(s => s.Expire());

        if (source)
        {
            signals.Add(gameObject.AddComponent<Signal>().SetSignal(sourcePower, powerCurve, ELECTRICITY_TICK_DURATION));
        }
    }

    public List<Electrode> RetrieveContacts()
    {
        return contacts;
    }

    public static int CompareElectrodes(Electrode a, Electrode b)
    {
        int aPriority = a.GetElectrodePriority();
        int bPriority = b.GetElectrodePriority();
        
        if (aPriority != bPriority) return (int) Mathf.Sign(aPriority - bPriority);
        
        return 0;
    }

    private int GetElectrodePriority()
    {
        if (source) return 0;
        else if (sender) return 5;
        else return 10;
    }

    public float GetConstant()
    {
        if(signals == null) return 0f;

        if(signals.Count <= 0) return 0f;

        float power = 0f;

        signals.ForEach(s => power += s.GetConstant());

        return power;
    }

    private void OnDestroy()
    {
        Electricity.Instance.RemoveComponentFromList(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;

        //Handles.Label(transform.position + new Vector3(0, 1, 0), $"Weight: {node.Weight}");
        //Handles.Label(transform.position + new Vector3(0, 1.2f, 0), $"Voltage: {power}");
    }
#endif
    
}
