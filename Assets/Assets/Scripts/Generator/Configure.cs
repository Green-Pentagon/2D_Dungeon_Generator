//TO DO:
//- double check that the grid snapper is snapping to the correct unit size.
//- Corridor functionality
//- wall-tile to corridor on corridor touch functionality
//- clean up dead ends (configurable?)
//- surround corridor tiles with walls functionality
//- currently, rooms spawn at z depth 0, add parameter to allow to configure this?


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configure : MonoBehaviour
{
    //Class attributes
    //----------------------------------
    public float UNIT_SIZE = 1.0f;
    public Sprite DebugFloorTile;
    public Sprite DebugWallTile;

    //determines the number of attempts that the algorithm will try to match the number of desired rooms for when placing.
    [HideInInspector]
    public int ATTEMPTS_ALLOWED = 300;

    //IF YOU CHANGE THESE VALUES, ALSO CHANGE THE VALUES OFR THE PUBLIC VARIABLES BELOW ACCORDINGLY.
    private GenerateRoom RoomGenScript;
    private SnapToGrid SnapToGrid;
    private int[] roomWidthRange = { 3, 100 };
    private int[] roomHeightRange = { 3, 100 };
    private Vector2 ConfinesCornerTopLeft = Vector2.zero;
    private Vector2 ConfinesCornerBottomRight = Vector2.zero;


    private List<GameObject> rooms;
    //----------------------------------
    //IN-EDITOR SLIDERS
    //----------------------------------

    //Random Seed Configuration
    [Header("Seed Settings", order = 0)]
    [Space(2, order = 1)]
    public int seed;
    public bool randomiseSeedOnStart = true;


    [Header("Room Settings: General", order = 2)]
    [Space(2, order = 3)]

    //room count sliders
    [Range(2,30), Tooltip("WARNING: High room counts may impact performance!")]
    public int numberOfRooms;

    //room width sliders
    [Range(3, 100)]
    public int minRoomWidth;
    [Range(3, 100)]
    public int maxRoomWidth;

    //room height sliders
    [Range(3, 100)]
    public int minRoomHeight;
    [Range(3, 100)]
    public int maxRoomHeight;


    [Header("Room Settings: Spread", order = 4)]
    [Space(2, order = 5)]
    //room spread settings
    [Range(0, 200)]
    public int spawnSpreadX;
    [Range(0, 200)]
    public int spawnSpreadY;

    //----------------------------------
    //IN-EDITOR SLIDER VALIDATION
    //----------------------------------
    [ExecuteInEditMode]
    void OnValidate()
    {
        if (minRoomWidth > maxRoomWidth && maxRoomWidth != roomWidthRange[1])
        {
            maxRoomWidth = minRoomWidth;
        }

        if (minRoomHeight > maxRoomHeight && maxRoomHeight != roomHeightRange[1])
        {
            maxRoomHeight = minRoomHeight;
        }
    }
    //----------------------------------
    //Class Methods
    //----------------------------------
    
    
    void EnableWallTileColliders(List<GameObject> roomList)
    {
        //goes through every tile in every room & enables all box colliders
        foreach (GameObject room in roomList)
        {
            //destroys the box-collider used for seperation.
            Destroy(room.GetComponent<BoxCollider2D>());
            //Destroy(room.GetComponent<Rigidbody2D>());

            //re-enables wall tile collisions.
            for (int i = 0; i < room.transform.childCount; i++)
            {
                if (room.transform.GetChild(i).TryGetComponent<BoxCollider2D>(out BoxCollider2D coll))
                {
                    //if a tile has a box collider, enable it
                    coll.enabled = true;
                }
            }
        }
    }

    bool IsClipping(ref GameObject givenRoom)
    {
        ContactFilter2D fltr = new ContactFilter2D();
        List<Collider2D> result = new List<Collider2D>();
        
        int numColliders = Physics2D.OverlapCollider(givenRoom.GetComponent<BoxCollider2D>(), fltr, result);

        return numColliders != 0;
    }


    void Start()
    {
        //(-x,y)
        ConfinesCornerTopLeft = new Vector2(-((maxRoomWidth * UNIT_SIZE) + (spawnSpreadX* UNIT_SIZE)) ,
                                            ((maxRoomHeight * UNIT_SIZE) + (spawnSpreadY * UNIT_SIZE)));
        //(x,-y)
        ConfinesCornerBottomRight = new Vector2(((maxRoomWidth * UNIT_SIZE) + (spawnSpreadX * UNIT_SIZE)) ,
                                                -((maxRoomHeight * UNIT_SIZE) + (spawnSpreadY * UNIT_SIZE)));

        RoomGenScript = GetComponentInParent<GenerateRoom>();
        SnapToGrid = GetComponentInParent<SnapToGrid>();

        if (randomiseSeedOnStart)
        {
            seed = (int)System.DateTime.Now.Ticks;
        }
        Random.InitState(seed);
        print("--DUNGEON GENERATOR SCRIPT START--\nSeed: " + seed);

        rooms = new List<GameObject>();
        int rndWidth;
        int rndHeight;
        Vector2 rndOffset;
        GameObject tempRoom;
        int cAttempt = 0;

        print("Attempting to generate "+ numberOfRooms + " number of rooms...");
        while (cAttempt < ATTEMPTS_ALLOWED && rooms.Count < numberOfRooms)
        {
            cAttempt++;

            rndWidth = Random.Range(minRoomWidth, maxRoomWidth + 1);
            rndHeight = Random.Range(minRoomHeight, maxRoomHeight + 1);
            rndOffset = new Vector2(Random.Range(-spawnSpreadX, spawnSpreadX), Random.Range(-spawnSpreadY, spawnSpreadY));
            tempRoom = RoomGenScript.CreateRoom(DebugFloorTile, DebugWallTile, UNIT_SIZE, rndWidth, rndHeight, rndOffset);

            if (!IsClipping(ref tempRoom))
            {
                rooms.Add(tempRoom);
            }
            else
            {
                Destroy(tempRoom);
            }
        }

        print("Generated " + rooms.Count + " rooms in " + cAttempt + " attempts.");

        SnapToGrid.Run(rooms, UNIT_SIZE);
        EnableWallTileColliders(rooms);


        //DEBUG FOR VISUALISING CONFINES
        //GameObject debugTile = new GameObject();
        //debugTile.name = "DebugTile";
        //debugTile.AddComponent<SpriteRenderer>();
        //debugTile.GetComponent<SpriteRenderer>().sprite = DebugFloorTile;
        //debugTile.GetComponent<Transform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //debugTile.transform.position = ConfinesCornerTopLeft;

        //GameObject debugTile2 = Instantiate(debugTile);
        //debugTile2.transform.position = ConfinesCornerBottomRight;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //----------------------------------
}
