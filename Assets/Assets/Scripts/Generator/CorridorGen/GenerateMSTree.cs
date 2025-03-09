//TO DO:
//- TEST THAT THE DICTIONARY CHANGE WORKS, COPY PASTE INTO OLDER CODE IF NEED TO TEST IT, WEEK 9 OF DATASTRUCTURES WORK
//- look into how to make an MS Tree
//- current methodolgy won't work in connecting all the rooms together



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

        ////start Breadth-first search
        while (nodeQueue.Count > 0)
        {
            curNode = nodeQueue.Dequeue();

            //if found end, break out of process
            if (curNode == END_ID)
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
                    nodeQueue.Enqueue(adjID);
                }
            }
        }

        //determine the degree of seperation
        //crawls backwards through the path
        if (found)
        {
            //Console.WriteLine("END = " + END);
            curNode = END_ID;
            while (curNode != -1)
            {
                roomGraph.GetNodeByID(curNode).RemoveAllEdgesExcluding(prevIDs[curNode]);
                Debug.Log(roomGraph.GetNodeByID(curNode).AdjList);
                curNode = prevIDs[curNode];
            }
        }



    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Exec(ref List<Room> rooms)
    {
        roomGraph = new GraphWeighted<int>();
        END_ID = rooms.Count - 1;
        PopulateGraph(ref rooms);


    }
}
