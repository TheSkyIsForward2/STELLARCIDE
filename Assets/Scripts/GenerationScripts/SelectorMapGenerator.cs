using System;
using System.Collections.Generic;
using System.IO;
using MapScripts;
using Newtonsoft.Json;
using NUnit.Framework;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

[Serializable]
public sealed class GenNode {
    public string Id;
    public string Type;              			// "Room", "DialogResponse", etc.  Could also be a String as Tag
    public int Width;
    public int Depth;
    public int Difficulty;
    public bool isPlayerPosition = false;
    public string Description;
    
    public GenNode(string id, string type, int width, int depth, int difficulty, string description)
    {
        Id = id;
        Type = type;
        Width = width;
        Depth = depth;
        Difficulty = difficulty;
        Description = description;
    }
}

[Serializable]
public class Root
{
    public List<GenNode> nodes;
    public List<GenEdge> edges;

    public Root(List<GenNode> nodes, List<GenEdge> edges)
    {
        this.nodes = nodes;
        this.edges = edges;
    }
}


[Serializable]
public sealed class GenEdge {
    public string From;
    public string To;

    public GenEdge(string from, string to)
    {
        From = from;
        To = to;
    }
}

[Serializable]
public sealed class GenGraph {
    public Dictionary<string, GenNode> Nodes = new();
    public List<GenEdge> Edges = new();
    public Dictionary<string, List<GenEdge>> Out = new();
    
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
    
    public GenGraph graph;
    public Root currentRoot;
    [SerializeField] private int maxDepth = 8;
    [SerializeField] private int maxWidth = 4;
    [SerializeField] private int minWidth = 2;

    public TextAsset fileToReadWrite;
    public string resourcePath = "SavedFiles/PlayerMapPosition";
    public string fullPathToFile = "Assets/Resources/SavedFiles/PlayerMapPosition.json";
    
    private void Start()
    {
        
        fileToReadWrite = Resources.Load<TextAsset>(resourcePath);
        currentRoot = ReadFromFile();
        Generate();
    }
    
    void Generate()
    {
        // SKIP IF ALREADY CREATED!!!!! Check through write to JSON?
        if (currentRoot.edges.Count == 0)
        {
            Random rand = new Random();
            GenNode root = new GenNode("0", "root", 0, 0, 0, "test description");
            root.isPlayerPosition = true;
            graph.AddNode(root);
            currentRoot.nodes.Add(root);
            int nodeId = 1;
            int difficulty = 0;

            // create "columns" of nodes with depth, randomly creating 1 to max_width number of nodes
            int columnWidth = minWidth;
            for (int depth = 1; depth < maxDepth - 1; depth++)
            {
                for (int width = 0; width < columnWidth; width++)
                {
                    difficulty = rand.Next(0, 5);
                    graph.AddNode(new GenNode($"{nodeId}", "Area", width, depth, difficulty, "test description"));
                    currentRoot.nodes.Add(new GenNode($"{nodeId}", "Area", width, depth, difficulty, "test description"));
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

            difficulty = 4;
            //create end node (id: ?, type: end, width: ?, depth: max_depth-1)
            graph.AddNode(new GenNode($"{nodeId}", "end", 0, maxDepth - 1, difficulty, "test description"));
            currentRoot.nodes.Add(new GenNode($"{nodeId}", "end", 0, maxDepth - 1, difficulty, "test description"));

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
                            currentRoot.edges.Add(new GenEdge(parents[0].Id, child.Id));
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
                            if (child.Width == parent.Width || child.Width == parent.Width + 1)
                            {
                                graph.AddEdge(new GenEdge(parent.Id, child.Id));
                                currentRoot.edges.Add(new GenEdge(parent.Id, child.Id));
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
                            if (child.Width == parent.Width || child.Width == parent.Width - 1)
                            {
                                graph.AddEdge(new GenEdge(parent.Id, child.Id));
                                currentRoot.edges.Add(new GenEdge(parent.Id, child.Id));
                            }
                        }
                    }
                }
            }

            // end node case
            GenNode endNode = graph.GetNode($"{nodeId}");
            foreach (GenNode parent in nodesByDepth[maxDepth - 2])
            {
                graph.AddEdge(new GenEdge(parent.Id, endNode.Id));
                currentRoot.edges.Add(new GenEdge(parent.Id, endNode.Id));
            }
        }
        else
        {
            // data of the JSON: Root -> List of Nodes and Edges
            // Step 1: grab nodes

            foreach (GenNode node in currentRoot.nodes)
            {
                graph.AddNode(node);
            }

            // Step 2: grab edges
            foreach (GenEdge edge in currentRoot.edges)
            {
                graph.AddEdge(edge);    
            }
        }
        WriteToFile(currentRoot);
        GetComponent<GraphUIRenderer>().DrawGraph(graph);
    }

    public void ChangePlayerLocation(string nodeId)
    {
        // modify the root and re-write json
        foreach (GenNode node in currentRoot.nodes)
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
        WriteToFile(currentRoot);
    }

    public Root ReadFromFile()
    {
        if (!File.Exists(fullPathToFile))
            return new Root(new List<GenNode>(), new List<GenEdge>());
        
        // break apart json into node and edges text
        string json = fileToReadWrite.text;
        
        currentRoot = JsonConvert.DeserializeObject<Root>(json);

        // debug prints
        /*foreach (GenNode node in currentRoot.nodes)
            print(node.Id);
        foreach(GenEdge edge in currentRoot.edges)
            print(edge.From + "->" + edge.To);
        */
        
        // reached end of tree, so start with a new tree.
        if(currentRoot.nodes[currentRoot.nodes.Count - 1].isPlayerPosition)
            return new Root(new List<GenNode>(), new List<GenEdge>());
        
        print("found existing graph");
        return currentRoot;
    }

    public void WriteToFile(Root root)
    {
        // create the serializer
        JsonSerializer serializer = new JsonSerializer();

        // use StreamWriter and JSONWriter to create or overwrite file
        StreamWriter sw = new StreamWriter(fullPathToFile, false);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, root);
        }
        sw.Close();
    }

    public void ClearJson()
    {
        if (File.Exists(fullPathToFile))
        {
            File.Delete(fullPathToFile);
        }
    }
}