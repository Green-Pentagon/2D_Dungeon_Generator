using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMazeCorridors : MonoBehaviour
{
    private List<GameObject> corridors = new List<GameObject>();
    private GameObject tempCorridor;
    private Vector2 BoundsTopLeft;
    private Vector2 BoundsBottomRight;
    private Sprite tile;
    private float UNIT;

    void SetBounds(Vector2 TopL, Vector2 BottomR)
    {
        BoundsTopLeft = TopL;
        BoundsBottomRight = BottomR;
    }

    void SetCorridorTile(Sprite cTile)
    {
        tile = cTile;
    }

    void PrepareCorridorObject()
    {
        //creating template floor tile
        tempCorridor.name = "CorridorTile";
        tempCorridor.AddComponent<SpriteRenderer>();
        tempCorridor.GetComponent<SpriteRenderer>().sprite = tile;
        tempCorridor.GetComponent<Transform>().localScale = new Vector3(1.0f * UNIT, 1.0f * UNIT, 1.0f);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exec(Vector2 TopLeftBound, Vector2 BottomRightBound, Sprite corridorTile, float gridUnit, int numCorridors)
    {
        SetBounds(TopLeftBound, BottomRightBound);
        SetCorridorTile(corridorTile);
        UNIT = gridUnit;
        PrepareCorridorObject();

        for (int i = 0; i < numCorridors; i++)
        {

        }

        Destroy(tempCorridor);
    }

}
