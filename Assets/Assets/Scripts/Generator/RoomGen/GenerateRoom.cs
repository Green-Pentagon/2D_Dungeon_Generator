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

        GameObject floorTile = new GameObject();
        floorTile.AddComponent<SpriteRenderer>();
        floorTile.GetComponent<SpriteRenderer>().sprite = FloorTile;
        floorTile.GetComponent<Transform>().lossyScale.Set(1.0f * unit, 1.0f * unit, 1.0f);

        GameObject tempFloor;
        
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
