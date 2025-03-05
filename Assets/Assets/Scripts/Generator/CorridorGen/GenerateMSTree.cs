//TO DO:
//- TEST THAT THE DICTIONARY CHANGE WORKS, COPY PASTE INTO OLDER CODE IF NEED TO TEST IT, WEEK 9 OF DATASTRUCTURES WORK
//- look into how to make an MS Tree
//- current methodolgy won't work in connecting all the rooms together



using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GenerateMSTree : MonoBehaviour
{
    // Start is called before the first frame update
    GraphWeighted<int> roomGraph;
    
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
        PopulateGraph(ref rooms);


    }
}
