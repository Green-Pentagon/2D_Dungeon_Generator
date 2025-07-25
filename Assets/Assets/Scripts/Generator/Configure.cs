using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Configure : MonoBehaviour
{
    //Class attributes
    //----------------------------------
    public GameObject PlayerPrefab;
    public float UnitSize = 1.0f;
    private float UNIT_SIZE;
    public Sprite DebugFloorTile;
    public Sprite DebugWallTile;

    //determines the number of attempts that the algorithm will try to match the number of desired rooms for when placing.
    [HideInInspector]
    public int ATTEMPTS_ALLOWED = 300;

    //IF YOU CHANGE THESE VALUES, ALSO CHANGE THE VALUES OFR THE PUBLIC VARIABLES BELOW ACCORDINGLY.
    private GameObject DungeonParentObj;
    private GenerateRoom RoomGenScript;
    private SnapToGrid SnapToGrid;
    private GenerateMSTree MSTreeGenScript;
    private PointToPointWalker PointToPointWalker;
    private int[] roomWidthRange = { 3, 100 };
    private int[] roomHeightRange = { 3, 100 };
    private Vector2 ConfinesCornerTopLeft = Vector2.zero;
    private Vector2 ConfinesCornerBottomRight = Vector2.zero;


    private List<Room> rooms;
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
    [Range(2, 30), Tooltip("WARNING: High room counts may impact performance!")]
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
        if (UnitSize <= 0)
        {
            UnitSize = 0.01f;
        }
        else if (UnitSize >= 100)
        {
            UnitSize = 100;
        }
        

        UNIT_SIZE = UnitSize;

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

    void SetConfines()
    {
        //(-x,y)
        ConfinesCornerTopLeft = new Vector2(-((maxRoomWidth * UNIT_SIZE) + (spawnSpreadX * UNIT_SIZE)),
                                            ((maxRoomHeight * UNIT_SIZE) + (spawnSpreadY * UNIT_SIZE)));
        //(x,-y)
        ConfinesCornerBottomRight = new Vector2(((maxRoomWidth * UNIT_SIZE) + (spawnSpreadX * UNIT_SIZE)),
                                                -((maxRoomHeight * UNIT_SIZE) + (spawnSpreadY * UNIT_SIZE)));

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

    void SetSeed()
    {
        if (randomiseSeedOnStart)
        {
            seed = (int)System.DateTime.Now.Ticks;
        }
        Random.InitState(seed);
    }
    void GenerateRooms()
    {
        rooms = new List<Room>();
        int rndWidth;
        int rndHeight;
        Vector2 rndOffset;
        Room tempRoom;
        int cAttempt = 0;

        print("Attempting to generate " + numberOfRooms + " number of rooms...");
        while (cAttempt < ATTEMPTS_ALLOWED && rooms.Count < numberOfRooms)
        {
            cAttempt++;

            rndWidth = Random.Range(minRoomWidth, maxRoomWidth + 1);
            rndHeight = Random.Range(minRoomHeight, maxRoomHeight + 1);
            rndOffset = new Vector2(Random.Range(-spawnSpreadX, spawnSpreadX), Random.Range(-spawnSpreadY, spawnSpreadY));
            

            tempRoom = RoomGenScript.CreateRoom(DebugFloorTile, DebugWallTile, UNIT_SIZE, rndWidth, rndHeight, rndOffset);

            if (!IsClipping(ref tempRoom.GetRoom()))
            {
                tempRoom.SetId(rooms.Count);
                tempRoom.GetRoom().transform.parent = DungeonParentObj.transform;
                rooms.Add(tempRoom);
            }
            else
            {
                DestroyImmediate(tempRoom.GetRoom());
                tempRoom = null;
            }
        }

        if (rooms.Count < numberOfRooms)
        {
            Debug.LogWarning("Generator fell short of target room count (" + numberOfRooms + " desired, " + rooms.Count + " created)");
        }

        print("Generated " + rooms.Count + " rooms in " + cAttempt + " attempts.");
    }

    void EnableWallTileColliders(List<Room> roomList)
    {
        //goes through every tile in every room & enables all box colliders
        foreach (Room room in roomList)
        {
            //DestroyImmediates the box-collider used for seperation.
            DestroyImmediate(room.GetRoom().GetComponent<BoxCollider2D>());
            //DestroyImmediate(room.GetComponent<Rigidbody2D>());

            //re-enables wall tile collisions.
            for (int i = 0; i < room.GetRoom().transform.GetChild(0).childCount; i++)
            {
                if (room.GetRoom().transform.GetChild(0).GetChild(i).TryGetComponent<BoxCollider2D>(out BoxCollider2D coll))
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


    public void Exec()
    {
        OnValidate();
        RoomGenScript = GetComponentInParent<GenerateRoom>();
        SnapToGrid = GetComponentInParent<SnapToGrid>();
        MSTreeGenScript = GetComponentInParent<GenerateMSTree>();
        PointToPointWalker = GetComponentInParent<PointToPointWalker>();

        long timeStart = System.DateTime.Now.Ticks;
        System.TimeSpan deltaTimeExec;

        SetConfines();
        SetSeed();

        print("--DUNGEON GENERATOR SCRIPT START--\nSeed: " + seed);
        print("confined to area: (" + ConfinesCornerTopLeft.x + ", " + ConfinesCornerTopLeft.y + ") to (" + ConfinesCornerBottomRight.x + ", " + ConfinesCornerBottomRight.y + ").");

        DungeonParentObj = new GameObject();
        DungeonParentObj.name = "Dungeon (seed: " + seed + ")";

        GenerateRooms();

        SnapToGrid.Run(rooms, UNIT_SIZE);
        EnableWallTileColliders(rooms);
        MSTreeGenScript.Exec(ref rooms);
        //MSTreeGenScript.debugDrawConnections(rooms);

        PointToPointWalker.Exec(UNIT_SIZE, DebugFloorTile,DebugWallTile, rooms, MSTreeGenScript.GetEdgeList(),ref DungeonParentObj);
        deltaTimeExec = new System.TimeSpan(System.DateTime.Now.Ticks - timeStart);
        
        print("--DUNGEON GENERATOR SCRIPT FINISHED (" + deltaTimeExec.TotalSeconds + " seconds)--");

        print("Spawning Player (if set in Config Script)...");
        if (PlayerPrefab != null)
        {
            GameObject Plr = Instantiate(PlayerPrefab);
            Plr.transform.position = (Vector3)rooms.ElementAt(0).GetRoomCentre() + Vector3.back;
        }


    }


    //----------------------------------
}
