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
    List<GameObject> corridorWallTiles;
    GameObject CorridorTile;
    GameObject WallTile;
    GameObject CorridorTileParent;
    GameObject WallTileParent;

    bool IsValidPlacement(ref Vector2 curPos, ref List<Room> rooms, bool isCorridorFloor)
    {
        foreach (Room room in rooms)
        {
            if (isCorridorFloor)
            {
                if (room.ConvertWallToFloorTile(curPos))
                {
                    return false;
                }
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

                if (IsValidPlacement(ref curPos, ref rooms,true))
                {
                    curTile = Instantiate(CorridorTile);
                    curTile.transform.position = curPos;
                    curTile.transform.parent = CorridorTileParent.transform;
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

                if (IsValidPlacement(ref curPos, ref rooms, true))
                {
                    curTile = Instantiate(CorridorTile);
                    curTile.transform.position = curPos;
                    curTile.transform.parent = CorridorTileParent.transform;
                    corridorTiles.Add(curTile);
                }
            }
        }
    }
    
    void GenerateWalls(ref List<Room> rooms)
    {
        //checks the neighbouring 8 tiles (moore neighbourhood) surrounding a corridor tile to determine if a wall can be placed there
        

        GameObject curTile;
        Vector2 curPos;
        Vector2 offset = new Vector2(UNIT, UNIT);

        //XY
        //AB,AC, BA, BB, BC, CA, CB, CC (AA is redundant since it is the tile's position)
        //A = 0
        //B = 1
        //C = -1
        // character determines how the offset should be multiplied for X and Y respectfully in order to be checked
        string checkingOrder = "ABACBABBBCCACBCC";

        foreach (GameObject corridor in corridorTiles)
        {
            curPos = corridor.transform.position;

            for (int i = 1; i <= checkingOrder.Length; i += 2)
            {
                Vector2 tempPos = Vector2.zero;

                //configure X
                switch (checkingOrder[i - 1])
                {
                    case 'B':
                        tempPos = Vector2.right * offset;
                        break;
                    case 'C':
                        tempPos = Vector2.left * offset;
                        break;
                }

                //configure Y
                switch (checkingOrder[i])
                {
                    case 'B':
                        tempPos = tempPos + (Vector2.up * offset);
                        break;
                    case 'C':
                        tempPos = tempPos + (Vector2.down * offset);
                        break;
                }

                //add the offset to the current position
                tempPos = curPos + tempPos;

                if (IsValidPlacement(ref tempPos, ref rooms,false))
                {
                    curTile = Instantiate(WallTile);
                    curTile.transform.position = tempPos;
                    curTile.transform.parent = WallTileParent.transform;
                    corridorWallTiles.Add(curTile);
                }

            }
        }
    }

    public void Exec(float unit,Sprite corridorTile,Sprite wallTile,List<Room> rooms, List<Tuple<int, int, float>> edgeList, ref GameObject parentObject)
    {
        edgesPositional = new List<Tuple<Vector2, Vector2>>();
        corridorTiles = new List<GameObject>();
        UNIT = unit;

        //instantiate corridor parent game object
        CorridorTileParent = new GameObject();
        CorridorTileParent.name = "Corridor Tiles";
        CorridorTileParent.transform.parent = parentObject.transform;

        //instantiate template corridor tile game object
        CorridorTile = new GameObject();
        CorridorTile.name = "Corridor";
        CorridorTile.AddComponent<SpriteRenderer>();
        CorridorTile.GetComponent<SpriteRenderer>().sprite = corridorTile;
        CorridorTile.GetComponent<Transform>().localScale = new Vector3(1.0f * unit, 1.0f * unit, 1.0f);



        //----------------------------------------------------------------
        Debug.Log("Populating PointToPoint Walker's pathing...");
        foreach (Tuple<int, int, float> edge in edgeList)
        {
            edgesPositional.Add(new Tuple<Vector2, Vector2>(rooms.ElementAt(edge.Item1).GetRoomCentre(), rooms.ElementAt(edge.Item1).GetRoomCentre() - rooms.ElementAt(edge.Item2).GetRoomCentre()));
            //edgesPositional.Add(new Tuple<int, int, Vector2>(edge.Item1, edge.Item2, rooms.ElementAt(edge.Item1).GetRoomCentre() + rooms.ElementAt(edge.Item2).GetRoomCentre()));
        }
        //----------------------------------------------------------------
        Debug.Log("Path list populated, beginning Walk...");
        Walk(ref rooms);
        
        //----------------------------------------------------------------

        //instantiate wall parent game object
        corridorWallTiles = new List<GameObject>();
        WallTileParent = new GameObject();
        WallTileParent.name = "Corridor Wall Tiles";
        WallTileParent.transform.parent = parentObject.transform;

        //instantiate wall template object
        WallTile = new GameObject();
        WallTile.name = "Wall";
        WallTile.AddComponent<SpriteRenderer>();
        WallTile.GetComponent<SpriteRenderer>().sprite = wallTile;
        WallTile.GetComponent<Transform>().localScale = new Vector3(1.0f * unit, 1.0f * unit, 1.0f);

        Debug.Log("Corridors generated, adding surrounding walls...");
        GenerateWalls(ref rooms);

        //----------------------------------------------------------------

        Debug.Log("Walker Processes Complete, Cleaning Up...");
        //DestroyImmediate tiles which intersect rooms and oneanother
        DestroyImmediate(CorridorTile);
        DestroyImmediate(WallTile);
    }
}
