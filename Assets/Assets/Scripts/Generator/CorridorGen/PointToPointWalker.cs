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
        foreach (var edge in edgesPositional)
        {
            curPos = edge.Item1;
            //infLoopCatcher = 0;

            Debug.Log("Walking from " + edge.Item1.ToString() + " amount " + edge.Item2.ToString());

            for (int i = 0; i < MathF.Abs(edge.Item2.x) / UNIT; i++)
            {
                if (edge.Item2.x < 0)
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

            for (int i = 0; i < MathF.Abs(edge.Item2.y) / UNIT; i++)
            {
                if (edge.Item2.y < 0)
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
        }
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
            edgesPositional.Add(new Tuple<Vector2, Vector2>(rooms.ElementAt(edge.Item1).GetRoomCentre(), rooms.ElementAt(edge.Item1).GetRoomCentre() - rooms.ElementAt(edge.Item2).GetRoomCentre()));
            //edgesPositional.Add(new Tuple<int, int, Vector2>(edge.Item1, edge.Item2, rooms.ElementAt(edge.Item1).GetRoomCentre() + rooms.ElementAt(edge.Item2).GetRoomCentre()));
        }
        Debug.Log("Path list populated, beginning Walk...");
        Walk();
        Debug.Log("Walk Complete, Cleaning Up...");
        //DestroyImmediate tiles which intersect rooms and oneanother
        DestroyImmediate(Tile);
    }
}
