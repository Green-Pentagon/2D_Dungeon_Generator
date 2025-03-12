//TO DO:
//- rewrite internals to use a min or a max heap instead of a linked list for storing.

//code re-used from previous github repo project
//https://github.com/Green-Pentagon/UNI_Year2_Algorithms_Unit

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GraphNode<T> where T : IComparable
{
    T id;
    bool visited = false;
    Dictionary<T, float> weightMap = new Dictionary<T, float>();

    //=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=
    public GraphNode(T id)
    {
        this.id = id;
        //adjList = new LinkedList<T>();
        //weightList = new LinkedList<float>();
    }
    public T ID { get { return id; } }
    public Dictionary<T,float>.KeyCollection AdjList { get { return weightMap.Keys; } }
    public Dictionary<T, float>.ValueCollection WeightList { get { return weightMap.Values; } }
    //=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=
    public void AddEdge(GraphNode<T> graphNode)
    {
        weightMap.TryAdd(graphNode.ID,-1.0f);
        //adjList.AddLast(graphNode.ID);
        //weightList.AddLast(-1.0f);//default weight value
    }
    public void AddEdge(GraphNode<T> graphNode, float weight)
    {
        weightMap.TryAdd(graphNode.ID, weight);
        //adjList.AddLast(graphNode.ID);
        //weightList.AddLast(weight);
    }

    public void RemoveEdge(T ToID)
    {
        weightMap.Remove(ToID);
    }

    public void RemoveAllEdgesExcluding(T ToID)
    {
        float temp;
        try
        {
            temp = weightMap[ToID];
        }
        catch {
            return;
        }
        
        weightMap.Clear();
        weightMap.Add(ToID, temp);

    }

    public void RemoveAllEdgesExcluding(T[] ToID)
    {
        Dictionary<T, float> temp = new Dictionary<T, float>();
        foreach (T ID in ToID)
        {
            try
            {
                temp.Add(ID,weightMap[ID]);
            }
            catch 
            {
                return;
            }
        }
        weightMap = temp;

    }

    public bool GetVisited() { return visited; }
    public void SetVisited(bool status) { visited = status; }
}

public class GraphWeighted<T> where T : IComparable
{
    LinkedList<GraphNode<T>> nodes;
    //=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=
    public GraphWeighted() { nodes = new LinkedList<GraphNode<T>>(); }
    //=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=
    public bool IsEmpty()
    {
        return nodes.Count == 0;
    }

    public void AddNode(T id) //appends node into LinkedList of nodes.
    {
        nodes.AddLast(new GraphNode<T>(id));
    }


    public bool Contains(T id) //returns if an ID of specified node exists
    {
        foreach (GraphNode<T> node in nodes)
        {
            if (node.ID.CompareTo(id) == 0) return true;
        }
        return false;
    }

    public GraphNode<T> GetNodeByID(T id)
    {
        foreach (GraphNode<T> node in nodes)
        {
            if (node.ID.CompareTo(id) == 0)
            {
                return node;
            }
        }
        return null;
    }
    public void AddEdge(T from, T to) //Applies a default weight value to the edge determined in the GraphNode class constructor.
    {

        GraphNode<T> n1 = GetNodeByID(from);
        GraphNode<T> n2 = GetNodeByID(to);


        if (n1 != null && n2 != null)
        {
            n1.AddEdge(n2);
        }
        else
            Console.WriteLine("Node/s not found in the graph. Cannot add the edge");

    }

    public void AddEdge(T from, T to, float weight)
    {
        GraphNode<T> n1 = GetNodeByID(from);
        GraphNode<T> n2 = GetNodeByID(to);


        if (n1 != null && n2 != null)
        {
            n1.AddEdge(n2, weight);
        }
        else
            Console.WriteLine("Node/s not found in the graph. Cannot add the edge");

    }

    public bool IsAdjacent(GraphNode<T> from, GraphNode<T> to)
    {
        //does the 'from' node have an edge connection stored with the ID matching the 'to' node 
        foreach (GraphNode<T> n in nodes)
        {
            if (n.ID.CompareTo(from.ID) == 0)
            {
                return from.AdjList.Contains(to.ID);
            }
        }

        return false;
    }

    public int NumNodes()
    {
        int counter = 0;
        foreach (GraphNode<T> node in nodes)
        {
            counter++;
        }
        return counter;
    }
    public int NumEdges()
    {
        //less efficienct (O(n) vs O(1)) than having a variable that increments every time a connection is added, but doesn't overlook the scenario where someone adds a connection directly to a node using the public getter function)
        int counter = 0;
        foreach (GraphNode<T> node in nodes)
        {
            counter += node.AdjList.Count;
        }
        return counter;
    }

    public float SumWeight()
    {
        float counter = 0.0f;
        foreach (GraphNode<T> node in nodes)
        {
            counter += node.WeightList.Sum();
        }
        return counter;
    }

    public List<T> Outgoing(int k)
    //returns a list of node IDs, which have k number of outgoing connections from themselves
    {
        List<T> list = new List<T>();
        foreach (GraphNode<T> node in nodes)
        {
            if (node.AdjList.Count == k)
            {
                list.Add(node.ID);
            }
        }
        return list;
    }

    public List<T> Ingoing(int k)
    {
        //returns a list of node IDs, which have a k number of connections going into them from the nodes in the graph (itself included)
        List<T> list = new List<T>();
        Dictionary<T, int> dict = new Dictionary<T, int>();

        //populate the dictionary with default values
        foreach (GraphNode<T> node in nodes)
        {
            dict.Add(node.ID, 0);
        }

        //add the number of incoming connections for each node in the graph.
        foreach (GraphNode<T> node in nodes)
        {
            foreach (T t in node.AdjList)
            {
                dict[t] += 1;
            }

        }

        //see if any incoming connections in the dictionary are equal to k.
        foreach (GraphNode<T> node in nodes)
        {
            if (dict[node.ID] == k)
            {
                list.Add(node.ID);
            }
        }

        return list;
    }

    public List<T> Ingoing(T id)
    {
        //returs a list of IDs corresponding to ingoing edges to the node passed as parameter.
        List<T> list = new List<T>();
        foreach (GraphNode<T> node in nodes)
        {
            if (node.AdjList.Contains(id))
            {
                list.Add(node.ID);
            }
        }
        return list;
    }

    public List<T> LargestOutGoing()
    {
        List<T> list = new List<T>();
        int largest = 0;

        foreach (GraphNode<T> node in nodes)
        {
            if (largest < node.AdjList.Count())
            {
                largest = node.AdjList.Count();
            }
        }

        foreach (GraphNode<T> node in nodes)
        {
            if (node.AdjList.Count == largest)
            {
                list.Add(node.ID);
            }
        }
        return list;
    }

    public List<T> LargestInGoing()
    {
        List<T> list = new List<T>();
        Dictionary<T, int> dict = new Dictionary<T, int>();
        int largest = 0;

        //add the number of incoming connections for each node in the graph.
        foreach (GraphNode<T> node in nodes)
        {
            dict.Add(node.ID, 0);
        }

        //add the number of incoming connections for each node in the graph.
        foreach (GraphNode<T> node in nodes)
        {
            foreach (T t in node.AdjList)
            {
                dict[t] += 1;
            }

        }

        //find the largest value of ingoing connections within the dictionary.
        foreach (GraphNode<T> node in nodes)
        {
            if (largest < dict[node.ID])
            {
                largest = dict[node.ID];
            }
        }

        //add all nodes which match the largest number of ingoing connections.
        foreach (GraphNode<T> node in nodes)
        {
            if (dict[node.ID] == largest)
            {
                list.Add(node.ID);
            }
        }

        return list;
    }

    public float AvgConnectivity()
    {
        // Return the result of Total Edge Count / Total Node Count...
        if (NumNodes() > 0)
        {
            return (float)NumEdges() / (float)NumNodes();
        }
        return -99.9f;
    }

    public float DegreeCentrality(T NodeID)
    {
        //Degree Centriality(i) = Number of nodes i is connected with / Node count - 1

        if (NumNodes() > 1)
        {
            return (float)GetNodeByID(NodeID).AdjList.Count / (float)(NumNodes() - 1);
        }
        return -99.9f;

    }

    public string DegreeCentralities()
    {
        //ID will not print if unable to be converted into string format
        string buffer = "";

        foreach (GraphNode<T> node in nodes)
        {
            try// in the event the node ID throws an exception when converted to string
            {
                buffer += "Node ID: " + node.ID.ToString();
            }
            catch
            {
                buffer += "Node ID: " + "X";
            }
            buffer += ", Degree Centrality: " + DegreeCentrality(node.ID) + "\n";
        }

        return buffer;
    }

    public T LowestEdge(T from)
    {
        T to = default(T); //What we'll want to return...
        float? weight = null; //Keep lowest weight, start with null

        for (int i = 0; i < GetNodeByID(from).AdjList.Count; i++)
        {
            if (GetNodeByID(from).WeightList.ElementAt(i).CompareTo(weight) < 0 || weight == null)//compare the element at index, if it is smaller than currently stored weight, store new weight.
            {
                weight = GetNodeByID(from).WeightList.ElementAt(i);
                to = GetNodeByID(from).AdjList.ElementAt(i);
            }
        }

        return to;
    }

    public float? LowestEdgeWeight(T from)
    {
        T to = default(T); //What we'll want to return...
        float? weight = null; //Keep lowest weight, start with null

        for (int i = 0; i < GetNodeByID(from).AdjList.Count; i++)
        {
            if (GetNodeByID(from).WeightList.ElementAt(i).CompareTo(weight) < 0 || weight == null)//compare the element at index, if it is smaller than currently stored weight, store new weight.
            {
                weight = GetNodeByID(from).WeightList.ElementAt(i);
                to = GetNodeByID(from).AdjList.ElementAt(i);
            }
        }

        return weight;
    }

    public void RemoveNode(T id)
    {
        if (Contains(id))//if node exists in the graph
        {
            List<T> connectedNodes = Ingoing(id);//collect all ingoing nodes
            foreach (T nodeID in connectedNodes)//for every node with connections to the node we wish to remove
            {
                GetNodeByID(nodeID).RemoveEdge(id);
                //GetNodeByID(nodeID).AdjList.Remove(id);//remove the edge connecting the nodes
            }
            nodes.Remove(GetNodeByID(id));//finally remove the node when all ingoing connections are cut.
        }
        else
        {
            Console.WriteLine("Node of matching ID not found in the graph");
        }
    }
    
    public void RemoveEdge(T To,T From)
    {
        if (Contains(To) && Contains(From))//if node exists in the graph
        {
            List<T> ToconnectedNodes = Ingoing(To);//collect all ingoing nodes
            List<T> FromconnectedNodes = Ingoing(From);//collect all ingoing nodes
            foreach (T nodeID in ToconnectedNodes)//for every node with connections to the node we wish to remove
            {
                GetNodeByID(nodeID).RemoveEdge(From);//remove the edge connecting the nodes
            }
            foreach (T nodeID in FromconnectedNodes)//for every node with connections to the node we wish to remove
            {
                GetNodeByID(nodeID).RemoveEdge(To);//remove the edge connecting the nodes
            }
            

        }
        else
        {
            Console.WriteLine("Node of matching ID not found in the graph");
        }
    }

}
