//IMPORTANT: Please do not change the name of the Rooms inside of this script, as it is checked inside of SnapToGrid.cs script within the Run(int unit) method
//this is only needed one time to alter te box colliders for the rooms used to seperate them.

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateRoom : MonoBehaviour
{
    // Start is called before the first frame update


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject CreateRoom(Sprite FloorTile, Sprite WallTile, float unit, int width, int height,Vector2 offset)
    {
        GameObject room = new GameObject();
        room.name = "Room";
        Vector2 tileBounds = FloorTile.bounds.size;


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
                    tempTile.GetComponent<Transform>().parent = room.GetComponent<Transform>();
                    tempTile = null;//check if this clears it
                }
                else
                {
                    //create floor tile
                    tempTile = Instantiate(floorTile);
                    tempTile.GetComponent<Transform>().position = new Vector3((x * unit), (y * unit), 0.0f);
                    tempTile.GetComponent<Transform>().parent = room.GetComponent<Transform>();
                    tempTile = null;//check if this clears it
                }
                
            }
        }

        Destroy(floorTile);
        Destroy(wallTile);

        room.GetComponent<Transform>().position = offset * unit;

        //Seperation Logic Collider & rigidbody, to be disabled and or removed later down the creation process
        //this collider is x units wider and taller than the room, as to ensure that the rooms will never touch.
        room.AddComponent<BoxCollider2D>();
        room.GetComponent<BoxCollider2D>().offset = new Vector2((width * unit) / 2.0f - unit/2.0f, (height * unit) / 2.0f - unit / 2.0f);
        
        //collider that matches the extents of the room, if it'll make tetection of patch touching easier, it could be reverted to this.
        //room.GetComponent<BoxCollider2D>().size = new Vector2(width * unit, height * unit);
        //collider needed in order to ensure that rooms are seperated.
        room.GetComponent<BoxCollider2D>().size = new Vector2(width * unit + (2f*unit), height * unit + (2f * unit));
        room.AddComponent<Rigidbody2D>();
        room.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        room.GetComponent<Rigidbody2D>().mass = 10.0f * (width+height);
        room.GetComponent<Rigidbody2D>().freezeRotation = true;
        room.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Discrete;


        return room;
    }
}
