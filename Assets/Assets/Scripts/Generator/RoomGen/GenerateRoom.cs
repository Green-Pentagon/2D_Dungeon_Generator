using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

public class Room : IComparable
{
    private float UNIT;
    private float width;
    private float height;
    private GameObject roomObj;
    private int roomId;
    private Vector2 roomAnchor;
    private Vector2 roomCentre;

    public Room(GameObject room, float _width, float _height, float unit)
    {
        UNIT = unit;
        roomObj = room;

        width = (int)((_width * UNIT) / UNIT);
        height = (int)((_height * UNIT) / UNIT);

        roomAnchor = room.transform.position;
        roomCentre = new Vector2(room.transform.position.x + (width/2.0f * UNIT),room.transform.position.y + (height / 2.0f * UNIT));

        if (roomCentre.x % UNIT != 0)
        {
            int temp = (int)(roomCentre.x / UNIT);
            roomCentre.x = temp*UNIT;
        }
        if (roomCentre.y % UNIT != 0)
        {
            int temp = (int)(roomCentre.y / UNIT);
            roomCentre.y = temp*UNIT;
        }
       
    }

    //public Room(GameObject room, int id)
    //{
    //    roomId = id;
    //    roomObj = room;
    //}

    public void SetId(int newId)
    {
        roomId = newId;
        roomObj.name += " " + newId;
    }
    public ref int GetRoomId() { return ref roomId; }
    public ref GameObject GetRoom() { return ref roomObj; }
    public Vector2 GetRoomCentre() { return roomCentre; }

    public Vector3 GetRoomPosition() { return roomAnchor; }

    public ref float GetWidth() { return ref  width; }
    public ref float GetHeight() { return ref  height; }

    public bool ConvertWallToFloorTile(Vector2 position)
    {


        if (Overlapping(position))
        {
            //for each wall object stored, see if its position matches
            for (int wallIndex = 0; wallIndex < roomObj.transform.GetChild(0).childCount;wallIndex++)
            {
                if (roomObj.transform.GetChild(0).GetChild(wallIndex).transform.position.x == position.x && roomObj.transform.GetChild(0).GetChild(wallIndex).transform.position.y == position.y)
                {
                    //convert wall into floor
                    roomObj.transform.GetChild(0).GetChild(wallIndex).GetComponent<SpriteRenderer>().sprite = roomObj.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite;
                    roomObj.transform.GetChild(0).GetChild(wallIndex).GetComponent<BoxCollider2D>().enabled = false;
                    //Debug.LogWarning(roomId + ": wall to floor convertion needed");
                    return true;
                    
                }
            }
        }
        return false;

        //check every wall if it matches the coordinates, if it does:
        //- remove its collision
        //- grab a copy of a sprite from existing floor tile & change its sprite to it
    }
    
    public bool Overlapping(Vector2 position)
    {
        //using the centre of the room causes important loss of information
        if ((roomAnchor.x + ((width-1) * UNIT)) >= position.x && (roomAnchor.x) <= position.x &&
            (roomAnchor.y + ((height-1) * UNIT)) >= position.y && (roomAnchor.y) <= position.y)
        {
            return true;
        }

        return false;
    }

    public int CompareTo(object other)
    {
        Room t = (Room)other;
        return roomId.CompareTo(t.roomId);
    }
}


public class GenerateRoom : MonoBehaviour
{

    public Room CreateRoom(Sprite FloorTile, Sprite WallTile, float unit, int width, int height,Vector2 offset)
    {
        GameObject room = new GameObject();
        room.name = "Room";
        Vector2 tileBounds = FloorTile.bounds.size;

        GameObject wallsParent = new GameObject();
        GameObject floorsParent = new GameObject();
        wallsParent.name = "Walls";
        floorsParent.name = "Floors";
        wallsParent.GetComponent<Transform>().parent = room.GetComponent<Transform>();
        floorsParent.GetComponent<Transform>().parent = room.GetComponent<Transform>();




        //creating template floor tile
        GameObject floorTile = new GameObject();
        floorTile.name = "FloorTile";
        floorTile.AddComponent<SpriteRenderer>();
        floorTile.GetComponent<SpriteRenderer>().sprite = FloorTile;
        floorTile.GetComponent<Transform>().localScale = new Vector3(1.0f * unit, 1.0f * unit, 1.0f);


        //creating template wall tile
        GameObject wallTile = new GameObject();
        wallTile.name = "WallTile";
        wallTile.AddComponent<SpriteRenderer>();
        wallTile.GetComponent<SpriteRenderer>().sprite = WallTile;
        wallTile.GetComponent<Transform>().localScale = new Vector3(1.0f * unit, 1.0f * unit, 1.0f);
        wallTile.AddComponent<BoxCollider2D>();
        //Due to how the rooms are being seperated, the colliders for the walls have to be disabled at this stage, as to avoid the physics engine from considering them in the calculations.
        wallTile.GetComponent<BoxCollider2D>().enabled = false;
        

        GameObject tempTile;
        
        //create tiles for the room
        for (int x = 0; x < width;x++)
        {
            for (int y = 0; y < height; y++)
            {
                if ((x == 0 || x == width-1) || (y == 0 || y == height - 1))
                {
                    //create wall tile
                    tempTile = Instantiate(wallTile);
                    tempTile.GetComponent<Transform>().position = new Vector3((x * unit), (y * unit), 0.0f);
                    tempTile.GetComponent<Transform>().parent = wallsParent.GetComponent<Transform>();
                    tempTile = null;//check if this clears it
                }
                else
                {
                    //create floor tile
                    tempTile = Instantiate(floorTile);
                    tempTile.GetComponent<Transform>().position = new Vector3((x * unit), (y * unit), 0.0f);
                    tempTile.GetComponent<Transform>().parent = floorsParent.GetComponent<Transform>();
                    tempTile = null;//check if this clears it
                }
                
            }
        }

        DestroyImmediate(floorTile);
        DestroyImmediate(wallTile);

        room.GetComponent<Transform>().position = offset * unit;

        //Seperation Logic Collider & rigidbody, to be disabled and or removed later down the creation process
        //this collider is x units wider and taller than the room, as to ensure that the rooms will never touch.
        room.AddComponent<BoxCollider2D>();
        room.GetComponent<BoxCollider2D>().offset = new Vector2((width * unit) / 2.0f - unit/2.0f, (height * unit) / 2.0f - unit / 2.0f);
        
        //collider that matches the extents of the room, if it'll make tetection of patch touching easier, it could be reverted to this.
        //room.GetComponent<BoxCollider2D>().size = new Vector2(width * unit, height * unit);
        //collider needed in order to ensure that rooms are seperated.
        room.GetComponent<BoxCollider2D>().size = new Vector2(width * unit + (2f*unit), height * unit + (2f * unit));
        

        return new Room(room,width,height,unit);
    }
}
