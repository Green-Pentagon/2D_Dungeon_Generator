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
        tempCorridor = new GameObject();
        tempCorridor.name = "CorridorTile";
        
        //--------

        tempCorridor.AddComponent<SpriteRenderer>();
        tempCorridor.GetComponent<SpriteRenderer>().sprite = tile;
        tempCorridor.GetComponent<Transform>().localScale = new Vector3(1.0f * UNIT, 1.0f * UNIT, 1.0f);
    }

    bool IsInBounds(Vector2 vec)
    {
        return !(vec.x > BoundsBottomRight.x || vec.x < BoundsTopLeft.x ||
                vec.y > BoundsTopLeft.y || vec.y < BoundsBottomRight.y);
    }



    bool PathValid(Vector3 Pos,ref Dictionary<Vector3,int> logOfPositions)
    {
        return !logOfPositions.ContainsKey(Pos);
    }

    bool Step(ref Vector2 curPos,ref Vector2 curForward, ref Dictionary<Vector3,int> logOfPos)
    {
        int directionRoll = Random.Range(1,5);
        switch (directionRoll)
        {
            case 1:
                if (IsInBounds(curPos + Vector2.up * UNIT))
                {
                    curForward = Vector2.up;
                }
                
                break;
            case 2:
                if (IsInBounds(curPos + Vector2.down * UNIT))
                {
                    curForward = Vector2.down;
                }
                break;
            case 3:
                if (IsInBounds(curPos + Vector2.left * UNIT))
                {
                    curForward = Vector2.left;
                }
                break;
            case 4:
                if (IsInBounds(curPos + Vector2.right * UNIT))
                {
                    curForward = Vector2.right;
                }
                break;
            default:
                Debug.LogError("RUNTIME ERROR: IMPOSSIBLE CONDITION MET IN Step METHOD WITHIN GenerateMazeCorridors.cs SCRIPT");
                break;
        }

        //check if place to step is valid
        if (IsInBounds(curPos + curForward* UNIT) && PathValid(curPos + curForward* UNIT ,ref logOfPos))
        {
            curPos += curForward * UNIT;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Exec(Vector2 TopLeftBound, Vector2 BottomRightBound,Vector2 StartPos, Sprite corridorTile, float gridUnit, int numCorridors)
    {
        //set up values
        SetBounds(TopLeftBound, BottomRightBound);
        SetCorridorTile(corridorTile);
        UNIT = gridUnit;
        PrepareCorridorObject();
        Vector2 cForward = Vector2.up;
        Dictionary<Vector3,int> posToIndexLog = new Dictionary<Vector3, int>();

        Vector2 cPos = new Vector2(StartPos.x,StartPos.y);

        //begin process
        for (int i = 0; i < numCorridors; i++)
        {
            //tile is valid
            if (Step(ref cPos,ref cForward,ref posToIndexLog))
            {
                corridors.Add(Instantiate(tempCorridor));
                corridors[corridors.Count-1].transform.position = cPos;
                posToIndexLog.Add(cPos,i);
            }
            
        }

        //clean-up
        Destroy(tempCorridor);
        print("Generated Maze with " + corridors.Count + " tiles.");
        return true;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
