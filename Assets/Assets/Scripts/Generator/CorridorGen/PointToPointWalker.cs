using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PointToPointWalker : MonoBehaviour
{
    float UNIT;
    List<Tuple<Vector2, Vector2>> edgesPositional = new List<Tuple<Vector2, Vector2>>();
    List<GameObject> corridorTiles = new List<GameObject>();
    GameObject Tile;

    void Walk()
    {
        Vector2 curPos;
        GameObject curTile;
        int infLoopCatcher = 0;
        int infLoopThreshold = 100;
        foreach (var edge in edgesPositional)
        {
            curPos = edge.Item1;
            infLoopCatcher = 0;

            Debug.Log("Walking from " + edge.Item1.ToString() + " to " + edge.Item2.ToString());

            while (((MathF.Abs(edge.Item2.x) - MathF.Abs(curPos.x)) > 0 && edge.Item1.x >=0) || ((MathF.Abs(edge.Item2.x) - MathF.Abs(curPos.x)) < 0 && edge.Item1.x < 0) || infLoopCatcher < infLoopThreshold)
            {
                infLoopCatcher++;
                if (edge.Item1.x < edge.Item2.x)
                {
                    curPos.x += UNIT;
                }
                else
                {
                    curPos.x -= UNIT;
                }
                curTile = Instantiate(Tile);
                curTile.transform.position = curPos;
                corridorTiles.Add(curTile);
            }

            if (infLoopCatcher == infLoopThreshold) { Debug.LogError("INF LOOP CATCHER FOR POINT TO POINT WALKER'S X VALUE TRIPPED"); }
            infLoopCatcher = 0;

            while (((MathF.Abs(edge.Item2.y) - MathF.Abs(curPos.y)) > 0 && edge.Item1.y >= 0) || ((MathF.Abs(edge.Item2.y) - MathF.Abs(curPos.y)) < 0 && edge.Item1.y < 0) || infLoopCatcher < infLoopThreshold)
            {
                infLoopCatcher++;
                if (edge.Item1.y < edge.Item2.y)
                {
                    curPos.y += UNIT;
                }
                else
                {
                    curPos.y -= UNIT;
                }
                curTile = Instantiate(Tile);
                curTile.transform.position = curPos;
                corridorTiles.Add(curTile);
            }
            if (infLoopCatcher == infLoopThreshold) { Debug.LogError("INF LOOP CATCHER FOR POINT TO POINT WALKER'S Y VALUE TRIPPED"); }
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exec(float unit,Sprite corridorTile,List<Room> rooms, List<Tuple<int, int, float>> edgeList)
    {
        UNIT = unit;

        Tile = new GameObject();
        Tile.name = "Corridor";
        Tile.AddComponent<SpriteRenderer>();
        Tile.GetComponent<SpriteRenderer>().sprite = corridorTile;
        Tile.GetComponent<Transform>().localScale = new Vector3(1.0f * unit, 1.0f * unit, 1.0f);


        Debug.Log("Populating PointToPoint Walker's pathing...");
        foreach (Tuple<int, int, float> edge in edgeList)
        {
            edgesPositional.Add(new Tuple<Vector2, Vector2>(rooms.ElementAt(edge.Item1).GetRoomCentre(), rooms.ElementAt(edge.Item2).GetRoomCentre()));
            //edgesPositional.Add(new Tuple<int, int, Vector2>(edge.Item1, edge.Item2, rooms.ElementAt(edge.Item1).GetRoomCentre() + rooms.ElementAt(edge.Item2).GetRoomCentre()));
        }
        Debug.Log("Path list populated, beginning Walk...");
        Walk();
        Debug.Log("Walk Complete, Cleaning Up...");
        //destroy tiles which intersect rooms
        Destroy(Tile);
    }
}
