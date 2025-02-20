using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Circuit : IDisposable
{
    private readonly int START_COUNT = int.MaxValue;
    
    delegate void PropagateAction(Node node, int layer, float elementCount, List<Node> nodesEvaluated);

    PropagateAction resetNodes = (Node node, int layer, float elementCount, List<Node> nodesEvaluated) => node.Component?.ResetPower();

    PropagateAction powerNodes = (Node node, int layer, float elementCount, List<Node> nodesEvaluated) => node.ContactNodes.ForEach(contact => {
        contact.BroadcastSpecific(node);
    });

    [SerializeField] private List<Node> nodes;

    public List<CancellationTokenSource> cancellationTokens { get; private set; }

    private Dictionary<Node, List<float>> weightsNodes;

    public List<Node> Elements { get { return nodes; } }

    public Circuit()
    {
        nodes = new List<Node>();

        cancellationTokens = new List<CancellationTokenSource>();

        weightsNodes = new Dictionary<Node, List<float>>();

        Electricity.Instance?.Circuits.Add(this);
    }

    public void AddNode(Node node)
    {
        if (nodes.Contains(node)) return;

        node.Circuit = this;
        nodes.Add(node);
    }

    public async void ResolveCircuit()
    {
        Electricity.Instance.FlushCircuitTasks(this);
        cancellationTokens.Clear();

        weightsNodes.Clear();

        nodes = SortNodes(nodes);
        nodes.ForEach(e => {
            e.ResetWeight();
            e.Component?.ResetPower();
        });

        if (nodes.Count <= 0) return;

        List<Node> nodesCopy = new List<Node>(nodes);

        List<Node> evaluatedNode = new List<Node>();

        int sources = 0;
        foreach (Node node in nodesCopy)
        {
            if(node.Component.Source)
            {
                evaluatedNode.Clear();
                nodesCopy.ForEach(n => n.ResetWeight());
                await PropagateRoutingForward(node, false, true, evaluatedNode);
                FlattenWeights(sources);
                sources++;
            }
        }

        ResolveWeghts();

        nodes.Sort((a, b) => CompareWeightElectrode(a, b));

        PowerCircuit();
    }

    private int CompareWeightElectrode(Node nodeA, Node nodeB)
    {
        if(nodeA.Weight < nodeB.Weight ) return -1;
        else if(nodeA.Weight > nodeB.Weight )return 1;
        else return 0;
    }

    private async Task PropagateForward(Node startNode, PropagateAction action, bool debug, bool useEvaluation, List<Node> evaluatedNode) 
    { 
        if (startNode == null) return;

        if (!nodes.Contains(startNode)) return;

        CancellationTokenSource cancelSource = new CancellationTokenSource();
        cancellationTokens.Add(cancelSource);

        await PropagateActionForward(startNode, action, 1, cancelSource.Token, debug);

        async Task PropagateActionForward(Node node, PropagateAction action, int layer, CancellationToken cancel, bool debug)
        {
            if(debug)Debug.Log("CALLING " + node.Component.name);
            action(node, layer, this.nodes.Count, evaluatedNode);
            evaluatedNode.Add(node);

            List<Node> nodes = new List<Node>(node.ContactNodes);

            foreach (Node nextNode in nodes)
            {
                if (node.Weight <= nextNode.Weight)
                {
                    continue;
                }

                if(evaluatedNode.Contains(nextNode) && useEvaluation)
                {
                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(.001f));

                await PropagateActionForward(nextNode, action, ++layer, cancel, debug);
            }
        }
    }

    private List<Node> SortNodes(List<Node> nodesList)
    {
        List<Node> openList = nodesList.FindAll(x => x.Component.Source);
        List<Node> closedList = new List<Node>();
        if (!openList.Any())
        {
            nodesList.Sort((a, b) => Electrode.CompareElectrodes(a.Component, b.Component));
            return new List<Node>(nodesList);
        }

        openList.ForEach(node => node.Weight = START_COUNT);
        
        nodesList.ForEach(node => node.ResetWeight());

        int maxIterations = 100;
        int iter = 0;
        var tagToAdd = new List<Node>();
        
        do
        {
            tagToAdd.Clear();
            
            foreach (var node in openList)
            {
                if(closedList.Contains(node)) continue;
                else closedList.Add(node);
                
                node.ContactNodes.ForEach(contact => {
                    if (node.Weight > contact.Weight)
                    {
                        contact.Weight = node.Weight - 1;
                        if(!openList.Contains(contact)) tagToAdd.Add(contact);
                    }
                });
            }
            
            openList.AddRange(tagToAdd);

            iter++;
        } while(closedList.Count < nodesList.Count && iter < maxIterations);

        return closedList;
    }

    private async Task PropagateRoutingForward(Node startNode, bool debug, bool useEvaluation, List<Node> evaluatedNode)
    {
        if (startNode == null) return;

        if (!nodes.Contains(startNode)) return;

        CancellationTokenSource cancelSource = new CancellationTokenSource();
        cancellationTokens.Add(cancelSource);

        await PropagateActionForward(startNode, 1, cancelSource.Token, debug);

        async Task PropagateActionForward(Node node, int layer, CancellationToken cancel, bool debug)
        {
            if (debug) Debug.Log("CALLING " + node.Component.name);
            RouteNode(node, layer, this.nodes.Count, evaluatedNode);
            evaluatedNode.Add(node);

            List<Node> nodes = new List<Node>(node.ContactNodes);
            
            if(node.Component.Device) return;
            
            foreach (Node nextNode in nodes)
            {
                if (evaluatedNode.Contains(nextNode) && useEvaluation)
                {
                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(.001f));

                await PropagateActionForward(nextNode, ++layer, cancel, debug);
            }
        }
    }

    public void RouteNode(Node node, int layer, float elementCount, List<Node> nodesEvaluated)
    {
        foreach(Node contact in node.ContactNodes)
        {
            if(!nodesEvaluated.Contains(contact) && !contact.Component.Source)
            {
                float value = Circuit.CalculateWeightPropagation(node, contact, elementCount, (float)layer, nodesEvaluated);
                if (node.Component.DebugTool || contact.Component.DebugTool) Debug.Log($"{node.Component.name} add {value} weight to {contact.Component.name}");

                contact.Weight += value;

                if(!weightsNodes.Keys.Contains(contact)) weightsNodes.Add((contact), new List<float> { value });
                else weightsNodes[contact].Add(value);
            }
        }
    }

    private void FlattenWeights(int start)
    {
        foreach(KeyValuePair<Node, List<float>> pair in weightsNodes)
        {
            if (pair.Value.Count <= start) continue;

            List<float> newWeights = new List<float>();

            for(int i = 0; i < start; i++)
            {
                newWeights.Add(pair.Value[i]);
            }

            float routeWeight = 0;
            for(int i = start; i < pair.Value.Count; i++)
            {
                routeWeight += pair.Value[i];
            }

            newWeights.Add(routeWeight);

            pair.Value.Clear();
            newWeights.ForEach(w => pair.Value.Add(w));
        }
    }

    private void ResolveWeghts()
    {
        foreach(KeyValuePair<Node, List<float>> pair in weightsNodes)
        {
            pair.Key.Weight = pair.Value.Max();
        }

        weightsNodes.Clear();
    }

    public void PowerCircuit()
    {
        List<Node> nodesCopy = new List<Node>(nodes);

        nodesCopy.Reverse();

        foreach (Node node in nodesCopy)
        {
            node.Broadcast();
        }
    }

    public static Circuit MergeCircuit(Circuit a, Circuit b)
    {
        Circuit merge = new Circuit();

        a.nodes.ForEach(n => merge.AddNode(n));
        b.nodes.ForEach(n => merge.AddNode(n));

        a.Dispose();
        b.Dispose();

        return merge;
    }

    public void Dispose()
    {
        if (Electricity.Instance.Circuits.Contains(this))
        {
            Electricity.Instance.Circuits.Remove(this);
        }
        Electricity.Instance.FlushCircuitTasks(this);

        cancellationTokens.Clear();

        nodes.Clear();

        weightsNodes.Clear();
    }

    public async void UpdateNodeForward(Node node)
    {
        if (!nodes.Contains(node)) return;

        await PropagateForward(node, resetNodes, false, false, new List<Node>());

        foreach (Node previousNode in node.ContactNodes)
        {
            previousNode.BroadcastSpecific(node);
        }

        await PropagateForward(node, powerNodes, false, false, new List<Node>());
    }

    public static float CalculateWeightPropagation(Node node, Node contact, float circuitElementCount, float layer, List<Node> nodesAlreadyEvaluated)
    {
        SwitchElectrode nodeSwitch = node.Component.gameObject.GetComponent<SwitchElectrode>();
        SwitchElectrode contactSwitch = contact.Component.gameObject.GetComponent<SwitchElectrode>();

        if(nodeSwitch != null)
        {
            if (!nodeSwitch.SwitchOn) return 0;
        }

        if(contactSwitch != null)
        {
            if (!contactSwitch.SwitchOn) return 0;
        }


        if (circuitElementCount <= 0) circuitElementCount = 1f;

        float decayConstant = .5f;
        float sourceDecay = node.Component.Source ? circuitElementCount * decayConstant : 1f;
        float nextNodes = GetNextRoutingNodes(node, nodesAlreadyEvaluated);
        float contactNodePreviousNodes = GetPreviousRoutingNodes(contact, nodesAlreadyEvaluated);

        var weightValue = (node.Weight - 1 / circuitElementCount) /
                          (nextNodes * contactNodePreviousNodes * sourceDecay);
        
        return Mathf.Max(0f, weightValue);
    }

    public static float GetNextRoutingNodes(Node node, List<Node> nodesAlreadyEvaluated)
    {
        float nextNodes = 0f;

        foreach (Node contacts in node.ContactNodes)
        {
            if (!nodesAlreadyEvaluated.Contains(contacts)) nextNodes++;
        }

        return nextNodes <= 0 ? 1f : nextNodes;
    }

    public static float GetPreviousRoutingNodes(Node node, List<Node> nodesAlreadyEvaluated)
    {
        float previousNodes = 0f;

        foreach (Node contacts in node.ContactNodes)
        {
            if (nodesAlreadyEvaluated.Contains(contacts)) previousNodes++;
        }

        return previousNodes <= 0 ? 1f : previousNodes;
    }
}
