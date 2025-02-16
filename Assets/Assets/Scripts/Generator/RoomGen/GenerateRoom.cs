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


    public Object CreateRoom(Sprite FloorTile, float unit, int width, int height)
    {
        GameObject room = new GameObject();
        room.name = "Room";

        //Seperation Logic Collider
        room.AddComponent<BoxCollider2D>();
        room.GetComponent<BoxCollider2D>().size = new Vector2(width*unit, height*unit);
        room.GetComponent<BoxCollider2D>().offset = new Vector2(unit, unit);
        room.AddComponent<Rigidbody2D>();
        room.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        room.GetComponent<Rigidbody2D>().freezeRotation = true;



        GameObject floorTile = new GameObject();
        floorTile.name = "FloorTile";
        floorTile.AddComponent<SpriteRenderer>();
        floorTile.GetComponent<SpriteRenderer>().sprite = FloorTile;
        floorTile.GetComponent<Transform>().localScale = new Vector3(1.0f * unit, 1.0f * unit, 1.0f);

        GameObject tempFloor;
        
        //create tiles for the room
        for (int x = 0; x < width;x++)
        {
            for (int y = 0; y < height; y++)
            {
                tempFloor = Instantiate(floorTile);

                tempFloor.GetComponent<Transform>().position = new Vector3((x*unit), (y * unit), 0.0f);
                tempFloor.GetComponent<Transform>().parent = room.GetComponent<Transform>();
                tempFloor = null;//check if this clears it
            }
        }

        Destroy(floorTile);

        return room;
    }
}
