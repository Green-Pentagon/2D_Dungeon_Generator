//TO DO:

//-either implement kurkark greedy algorithm or:

//1. find unconnected clusters
//2. for every cluster, connect rooms with lowest weights per cluster.
//3. join connected clusters & repeat until all united.
//result might not be the most optimal and might not be a perfect MS tree but should suffice if implementing greedy algorithm fails.


// - actually make the tree into an MS Tree (delunrey triangulation or whatever it was called)
// - alternatively, you can convert the tree into an MS tree by:
//      - Checking that all outgoing connections from every room ID is 1
//      - If not, remove the larger weighted connection
//      - go through the tree again and connect any isolated clusters & unite them into a single tree (tougher than it sounds)
// - Could further literacy review by re-adapting potential discovered methods of making an MS Tree in the future.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GenerateMSTree : MonoBehaviour
{
    // Start is called before the first frame update
    GraphWeighted<int> roomGraph;
    GraphWeighted<int> NodeToNodeWeights;
    const int START_ID = 0;
    int END_ID;
    
    void PopulateGraph(ref List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            roomGraph.AddNode(room.GetRoomId());
            NodeToNodeWeights.AddNode(room.GetRoomId());
        }

        foreach (Room room in rooms)
        {

            foreach (Room room2 in rooms)
            {
                if (room.GetRoomId() != room2.GetRoomId())
                {
                    roomGraph.AddEdge(room.GetRoomId(), room2.GetRoomId(), MathF.Abs((room.GetRoomCentre() - room2.GetRoomCentre()).magnitude));
                    NodeToNodeWeights.AddEdge(room.GetRoomId(), room2.GetRoomId(), MathF.Abs((room.GetRoomCentre() - room2.GetRoomCentre()).magnitude));
                }
            }

        }
    }

    void JoinFragmentedGraph()
    {
        int numNodes = roomGraph.NumNodes();
        List<Tuple<int,int,float>> edgeList = roomGraph.GetEdges();
        bool[] visited = new bool[numNodes];
        List<List<int>> clusters = new List<List<int>>();

        //creates and populates adj list for search.
        List<int>[] adjList = new List<int>[numNodes];
        for (int i = 0; i < numNodes; i++)
        {
            adjList[i] = new List<int>();
        }
        foreach (var e in edgeList)
        {
            int v0 = e.Item1;
            int v1 = e.Item2;
            adjList[v0].Add(v1);
            adjList[v1].Add(v0);
        }

        //initialise the variables for search
        Stack<int> nodeStack = new Stack<int>();
        List<int> CurrentCluster;
        int largestCluster = 0;
        int curNode;
        int nodeCount = 0;

        //while not every node has been checked
        while (nodeCount < visited.Length)
        {
            int temp = 0;
            //Find the next non-visited node
            while (visited[temp])
            {
                temp++;
            }
            //reset the current friend group & push the first person to check onto the stack.
            CurrentCluster = new List<int>();
            nodeStack.Push(temp);
            visited[temp] = true;

            //while stack isn't empty, commence Deph-First search
            while (nodeStack.Count > 0)
            {
                //node counter goes up every time a node is visited
                //unless a node connects to itself, no duplicate node should end up in the CurrentFriendGroup, an exception is thrown if so.
                curNode = nodeStack.Pop();
                nodeCount++;
                CurrentCluster.Add(curNode);
                //for every connection in the Adjacency list
                foreach (var vert in adjList[curNode])
                {
                    //if a node wasn't visited, add it to the stack to visit & set it as visited
                    if (!visited[vert])
                    {
                        visited[vert] = true;
                        nodeStack.Push(vert);
                    }
                }
            }

            //if the resulting cluster of connected nodes has more members than the currently recorded highest count, replace it.
            if (CurrentCluster.Count > largestCluster)
            {
                largestCluster = CurrentCluster.Count;
            }

            clusters.Add(CurrentCluster);
        }


        //connects clusters by lowest possible weight values, not checking for which cluster is closest to one another.

        Dictionary<int,float> cDict = new Dictionary<int,float>();

        float lowestWeightConnection;
        Tuple<int, int> lowestConnection;

        for (int i = 1; i < clusters.Count; i++)
        {
            lowestWeightConnection = float.MaxValue;
            lowestConnection = new Tuple<int, int>(-1,-1);

            foreach (int id1 in clusters[i])
            {
                cDict = NodeToNodeWeights.GetNodeByID(id1).WeightMap;

                foreach (int id2 in clusters[i - 1])
                {
                    if (lowestWeightConnection > NodeToNodeWeights.GetNodeByID(id1).WeightMap[id2])
                    {
                        lowestWeightConnection = NodeToNodeWeights.GetNodeByID(id1).WeightMap[id2];
                        lowestConnection = new Tuple<int,int>(id1, id2);
                    }
                }
            }
            roomGraph.AddEdge(lowestConnection.Item1, lowestConnection.Item2,lowestWeightConnection);
        }

    }

    void ConvertIntoMSTree() {

        //initialise the variables for search
        Queue<int> nodeQueue = new Queue<int>();
        int curNode;

        HashSet<int> Visited = new HashSet<int>();
        Visited.Add(START_ID);
        int[] prevIDs = new int[END_ID+1]; // build path from refs to previous
        prevIDs[START_ID] = -1; //mark the start for the crawl back to find the depth
        //int degreeSeperation = -1;
        bool found = false;

        nodeQueue.Enqueue(START_ID);

        //start Breadth-first search
        while (nodeQueue.Count > 0)
        {
            curNode = nodeQueue.Dequeue();

            //for each adjList connection, queue any unvisited nodes to visit
            foreach (int adjID in roomGraph.GetNodeByID(curNode).AdjList)
            {
                if (!roomGraph.GetNodeByID(adjID).GetVisited() && !Visited.Contains(adjID))
                {
                    //Console.WriteLine("pushing " + adjID);
                    roomGraph.GetNodeByID(adjID).SetVisited(true);
                    //nodesVisited[adjID] = true;
                    prevIDs[adjID] = curNode;
                    Visited.Add(adjID);
                    nodeQueue.Enqueue(adjID);

                    //Culls all other connected edges
                    int LowestWeightID = roomGraph.LowestEdge(curNode);
                    roomGraph.GetNodeByID(curNode).RemoveAllEdgesExcluding(LowestWeightID);
                    roomGraph.GetNodeByID(LowestWeightID).RemoveEdge(curNode);
                    
                    break;
                }

            }
        }

        found = true;

        //determine the degree of seperation
        //crawls backwards through the path
        if (found)
        {
            //Console.WriteLine("END = " + END);
            curNode = END_ID;
            while (curNode != -1)
            {
                roomGraph.GetNodeByID(curNode).RemoveAllEdgesExcluding(roomGraph.LowestEdge(curNode));
                //Debug.Log(curNode + " connects to " + roomGraph.GetNodeByID(curNode).AdjList.ElementAt(0) + " with a weight value " + roomGraph.GetNodeByID(curNode).WeightList.ElementAt(0));
                //Debug.Log(curNode + " contains " + roomGraph.GetNodeByID(curNode).AdjList.Count + " connections");
                curNode--;
                //curNode = prevIDs[curNode];
            }
        }



    }
    
    bool DebugIsEveryRoomReachable()
    {
        //bool output = true;
        //int counter = 1;


        //    //initialise the variables for search
        //    Stack<int> nodeStack = new Stack<int>();
        //    int curNode;

        //    HashSet<int> Visited = new HashSet<int>();
        //    Visited.Add(START_ID);
        //    int[] prevIDs = new int[END_ID + 1]; // build path from refs to previous
        //    prevIDs[START_ID] = -1; //mark the start for the crawl back to find the depth
        //                            //int degreeSeperation = -1;

        //    nodeStack.Push(START_ID);

        //    //start Breadth-first search
        //    while (nodeStack.Count > 0)
        //    {
        //        curNode = nodeStack.Pop();
                
        //        //if found end, break out of process
                

        //        //for each adjList connection, queue any unvisited nodes to visit
        //        foreach (int adjID in roomGraph.GetNodeByID(curNode).AdjList)
        //        {
        //            if (!roomGraph.GetNodeByID(adjID).GetVisited() && !Visited.Contains(adjID))
        //            {
        //                //Console.WriteLine("pushing " + adjID);
        //                counter++;
        //                roomGraph.GetNodeByID(adjID).SetVisited(true);
        //                //nodesVisited[adjID] = true;
        //                prevIDs[adjID] = curNode;
        //                Visited.Add(adjID);
        //                nodeStack.Push(adjID);
        //            }
        //        }
        //    }

        //    if (counter == roomGraph.NumNodes())
        //    {
        //        return false;
        //    }
        
        

        return false;
    }


    public void debugDrawConnections(List<Room> rooms)
    {
        int nID = -1;

        Vector2 prevRoomCentre = rooms.ElementAt(0).GetRoomCentre();

        for (int cID = 0; cID < roomGraph.NumNodes(); cID++)
        {
            nID = roomGraph.GetNodeByID(cID).AdjList.ElementAt(0);
            Debug.DrawLine(rooms.ElementAt(cID).GetRoomCentre(), rooms.ElementAt(nID).GetRoomCentre(), Color.red, 10.0f);
        }
    }

    public List<Tuple<int, int, float>> GetEdgeList()
    {
        return roomGraph.GetEdges(); 
    }

    public void Exec(ref List<Room> rooms)
    {
        roomGraph = new GraphWeighted<int>();
        NodeToNodeWeights = new GraphWeighted<int>();
        END_ID = rooms.Count - 1;
        Debug.Log("Generating a full graph...");
        PopulateGraph(ref rooms);
        Debug.Log("Converting Graph into MS Tree...");
        ConvertIntoMSTree();
        JoinFragmentedGraph();
        //Debug.Log("Is every room reachable from roomID 0 = " + DebugIsEveryRoomReachable());
        //Debug.Log("Adding % chance for loop in graph?"); // could also do this inside of the MS tree method with a probability chance governed by seed.

    }

    public ref GraphWeighted<int> GetGraph()
    {
        return ref roomGraph;
    }
}
