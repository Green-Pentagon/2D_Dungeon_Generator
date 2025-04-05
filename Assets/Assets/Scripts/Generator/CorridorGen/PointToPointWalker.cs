using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PointToPointWalker : MonoBehaviour
{
    float UNIT;
    List<Tuple<Vector2, Vector2>> edgesPositional;
    List<GameObject> corridorTiles;
    GameObject Tile;
    GameObject TileParent;

    bool IsValidPlacement(ref Vector2 curPos, ref List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            if (room.ConvertWallToFloorTile(curPos))
            {
                return false;
            }

            if (room.Overlapping(curPos))
            {
                //Debug.LogWarning("room (ID:" + room.GetRoomId()+ ") ("+ room.GetWidth() +" x " + room.GetHeight() +") overlap detected on" + curPos);
                return false;
            }
        }

        
        //check for pre-existing corridor tiles in this position
        //this is done in O(n), but could be reduced to approx. O(log n) via binary search.
        foreach (GameObject tile in corridorTiles)
        {
            if (curPos == (Vector2)tile.transform.position)
            {
                return false;
            }
        }

        return true;
        
    }


    void Walk(ref List<Room> rooms)
    {
        Vector2 curPos;
        GameObject curTile;
        foreach (var edge in edgesPositional)
        {
            curPos = edge.Item1;
            //Debug.Log("Walking from " + edge.Item1.ToString() + " amount " + edge.Item2.ToString());

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

                if (IsValidPlacement(ref curPos, ref rooms))
                {
                    curTile = Instantiate(Tile);
                    curTile.transform.position = curPos;
                    curTile.transform.parent = TileParent.transform;
                    corridorTiles.Add(curTile);
                }
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

                if (IsValidPlacement(ref curPos, ref rooms))
                {
                    curTile = Instantiate(Tile);
                    curTile.transform.position = curPos;
                    curTile.transform.parent = TileParent.transform;
                    corridorTiles.Add(curTile);
                }
            }
        }
    }
    
    public void Exec(float unit,Sprite corridorTile,List<Room> rooms, List<Tuple<int, int, float>> edgeList, ref GameObject parentObject)
    {
        edgesPositional = new List<Tuple<Vector2, Vector2>>();
        corridorTiles = new List<GameObject>();
        UNIT = unit;
        TileParent = new GameObject();
        TileParent.name = "Corridor Tiles";
        TileParent.transform.parent = parentObject.transform;

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
        Walk(ref rooms);
        Debug.Log("Walk Complete, Cleaning Up...");
        //DestroyImmediate tiles which intersect rooms and oneanother
        DestroyImmediate(Tile);
    }
}
