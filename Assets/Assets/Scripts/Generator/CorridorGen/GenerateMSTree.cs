//TO DO:
//- TEST THAT THE DICTIONARY CHANGE WORKS, COPY PASTE INTO OLDER CODE IF NEED TO TEST IT, WEEK 9 OF DATASTRUCTURES WORK
//- look into how to make an MS Tree
//- current methodolgy won't work in connecting all the rooms together



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
    const int START_ID = 0;
    int END_ID;
    
    void PopulateGraph(ref List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            roomGraph.AddNode(room.GetRoomId());
        }

        foreach (Room room in rooms)
        {

            foreach (Room room2 in rooms)
            {
                if (room.GetRoomId() != room2.GetRoomId())
                {
                    roomGraph.AddEdge(room.GetRoomId(), room2.GetRoomId(), (room.GetRoomCentre() + room2.GetRoomCentre()).magnitude);
                }
            }

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

            //if found end, break out of process
            //if (curNode == END_ID)
            //{
            //    found = true;
            //    break;
            //}

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
                    roomGraph.GetNodeByID(curNode).RemoveAllEdgesExcluding(adjID);
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
                Debug.Log(curNode + " connects to " + roomGraph.GetNodeByID(curNode).AdjList.ElementAt(0) + " with a weight value " + roomGraph.GetNodeByID(curNode).WeightList.ElementAt(0));
                Debug.Log(curNode + " contains " + roomGraph.GetNodeByID(curNode).AdjList.Count + " connections");
                curNode--;
                //curNode = prevIDs[curNode];
            }
        }



    }
    
    bool DebugIsEveryRoomReachable()
    {
        bool output = true;
        int cID = START_ID;


            //initialise the variables for search
            Stack<int> nodeStack = new Stack<int>();
            int curNode;

            HashSet<int> Visited = new HashSet<int>();
            Visited.Add(START_ID);
            int[] prevIDs = new int[END_ID + 1]; // build path from refs to previous
            prevIDs[START_ID] = -1; //mark the start for the crawl back to find the depth
                                    //int degreeSeperation = -1;
            bool found = false;

            nodeStack.Push(START_ID);

            //start Breadth-first search
            while (nodeStack.Count > 0)
            {
                curNode = nodeStack.Pop();

                //if found end, break out of process
                if (curNode == cID)
                {
                    found = true;
                    break;
                }

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
                        nodeStack.Push(adjID);
                    }
                }
            }

            if (!found)
            {
                return false;
            }
        
        

        return output;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exec(ref List<Room> rooms)
    {
        roomGraph = new GraphWeighted<int>();
        END_ID = rooms.Count - 1;
        Debug.Log("Generating a full graph...");
        PopulateGraph(ref rooms);
        Debug.Log("Converting Graph into MS Tree...");
        ConvertIntoMSTree();
        Debug.Log("Is every room reachable from roomID 0 = " + DebugIsEveryRoomReachable());
        //Debug.Log("Adding % chance for loop in graph?"); // could also do this inside of the MS tree method with a probability chance governed by seed.

    }
}
