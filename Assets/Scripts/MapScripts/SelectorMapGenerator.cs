using System.Collections.Generic;
using MapScripts;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;


public sealed class GenNode {
    public string Id;
    public string Type;              			// "Room", "DialogResponse", etc.  Could also be a String as Tag
    public int Width;
    public int Depth;
    public bool isPlayerPosition = false;
    
    public GenNode(string id, string type, int width, int depth)
    {
        Id = id;
        Type = type;
        Width = width;
        Depth = depth;
    }
}

public sealed class GenEdge {
    public string From;
    public string To;

    public GenEdge(string from, string to)
    {
        From = from;
        To = to;
    }
}

public sealed class GenGraph {
    public Dictionary<string, GenNode> Nodes = new();
    public List<GenEdge> Edges = new();
    public Dictionary<string, List<GenEdge>> Out = new();
    // adjacency list: maps a node ID → all outgoing edges (valid transitions)

    public void AddNode(GenNode node)
    {
        Nodes.Add(node.Id, node);
        Out[node.Id] = new List<GenEdge>();
    }

    public void AddEdge(GenEdge edge)
    {
        Edges.Add(edge);
        Out[edge.From].Add(edge);
    }

    public GenNode GetNode(string id)
    {
        return Nodes[id];
    }
}

public class SelectorMapGenerator : MonoBehaviour
{
    
    private GenGraph graph;
    [SerializeField] private int maxDepth = 8;
    [SerializeField] private int maxWidth = 4;
    [SerializeField] private int minWidth = 2;

    private void Start()
    {
        Generate();
    }
    
    void Generate()
    {
        // SKIP IF ALREADY CREATED!!!!! Check through write to JSON?
        if (graph != null)
            return;
        Random rand = new Random();
        graph = new GenGraph();
        GenNode root = new GenNode("0", "root", 0, 0);
        root.isPlayerPosition = true;
        graph.AddNode(root);
        int nodeId = 1;
        
        // create "columns" of nodes with depth, randomly creating 1 to max_width number of nodes
        int columnWidth = minWidth;
        for (int depth = 1; depth < maxDepth - 1; depth++)
        {
            for (int width = 0; width < columnWidth; width++)
            {
                graph.AddNode(new GenNode($"{nodeId}", "Area", width, depth));
                nodeId++;
            }

            if (columnWidth == minWidth)
            {
                columnWidth++;
            }
            else if (columnWidth == maxWidth)
            {
                columnWidth--;
            }
            else
            {
                columnWidth += rand.Next(2) > 0 ? 1 : -1;
            }
        }
        //create end node (id: ?, type: end, width: ?, depth: max_depth-1)
        graph.AddNode(new GenNode($"{nodeId}", "end", 0, maxDepth-1));
        
        // re-organize nodes based on depth
        Dictionary<int, List<GenNode>> nodesByDepth = new Dictionary<int, List<GenNode>>();
        foreach (GenNode node in graph.Nodes.Values)
        {
            if (!nodesByDepth.TryGetValue(node.Depth, out var list))
            {
                list = new List<GenNode>();
                nodesByDepth[node.Depth] = list;
            }

            list.Add(node);
        }
        
        // add edges to nodes based on depth
        for (int depth = 0; depth < maxDepth - 2; depth++)
        {
            if (!nodesByDepth.TryGetValue(depth, out var parents))
                continue;
            if (!nodesByDepth.TryGetValue(depth + 1, out var children))
            {
                continue;
            }

            // root node case
            if (depth == 0)
            {
                foreach (GenNode child in children)
                {
                    graph.AddEdge(new GenEdge(parents[0].Id, child.Id));
                }

                continue;
            }

            // Link to width-adjacent children
            if (parents.Count < children.Count)
            {
                foreach (GenNode parent in parents)
                {
                    foreach (GenNode child in children)
                    {
                        if (child.Width == parent.Width || child.Width == parent.Width+1)
                        {
                            graph.AddEdge(new GenEdge(parent.Id, child.Id));
                        }
                    }
                }
            }
            else if (depth != maxDepth - 2)
            {
                foreach (GenNode parent in parents)
                {
                    foreach (GenNode child in children)
                    {
                        if (child.Width == parent.Width || child.Width == parent.Width-1)
                        {
                            graph.AddEdge(new GenEdge(parent.Id, child.Id));
                        }
                    }
                }
            }
        }
        
        // end node case
        GenNode endNode = graph.GetNode($"{nodeId}");
        foreach (GenNode parent in nodesByDepth[maxDepth-2])
        {
            graph.AddEdge(new GenEdge(parent.Id, endNode.Id));
        }
        
        GetComponent<GraphUIRenderer>().DrawGraph(graph);
    }

    public void ChangePlayerLocation(string nodeId)
    {
        foreach (GenNode node in graph.Nodes.Values)
        {
            if (node.isPlayerPosition)
            {
                node.isPlayerPosition = false;
            }

            if (node.Id == nodeId)
            {
                node.isPlayerPosition = true;
                print($"position changed to {nodeId}");
            }
        }
    }
}
