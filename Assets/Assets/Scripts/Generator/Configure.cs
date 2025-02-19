//TO DO:
//- double check that the grid snapper is snapping to the correct unit size.
//- (uncomment the code & debug it so it works properly, needs re-writing to simplify it) ADD A CHECK TO ENSURE THAT ROOMS ARE NOT CLIPPING IN EACH OTHER AFTER THE COOLDOWN
//- Add the ability to select how you want to spread the rooms out & the size of the spread
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

    //IF YOU CHANGE THESE VALUES, ALSO CHANGE THE VALUES OFR THE PUBLIC VARIABLES BELOW ACCORDINGLY.
    private GenerateRoom RoomGenScript;
    private SnapToGrid SnapToGrid;
    private int[] roomWidthRange = { 3, 100 };
    private int[] roomHeightRange = { 3, 100 };
    private bool snapReady = false;
    float waitBeforeSnapping;
    //private int[] roomCountRange = { 1, 6 };

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
    [Range(1, 10)]
    //[Min(1), Tooltip("WARNING: High room counts may impact performance!")]
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
            Destroy(room.GetComponent<Rigidbody2D>());

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

    bool AreRoomsClipping()
    {
        bool output = false;
        //ContactFilter2D fltr = new ContactFilter2D();
        //List<Collider2D> result = new List<Collider2D>();
        //int numColliders = 0;
        foreach(GameObject room in rooms)
        {
            //numColliders = Physics2D.OverlapCollider(room.GetComponent<BoxCollider2D>(), fltr, result);
            //foreach(Collider2D coll in result)
            //{
            //    if (Mathf.Abs(Physics2D.Distance(room.GetComponent<BoxCollider2D>(),coll).distance) < room.GetComponent<Transform>().lossyScale.x ||
            //        Mathf.Abs(Physics2D.Distance(room.GetComponent<BoxCollider2D>(), coll).distance) < room.GetComponent<Transform>().lossyScale.y)
            //    {
            //        output = true;
            //        print("Clipping detected: " + numColliders + " colliders intersecting with a room collider.");
            //        return output;
            //    }
            //}

        }
        return output;
    }

    IEnumerator WaitForPhysicsSeperation()
    {
        bool roomsInvalid = true;

        while (roomsInvalid)
        {
            yield return new WaitForSeconds(waitBeforeSnapping);
            roomsInvalid = AreRoomsClipping();
            if (roomsInvalid)
            {
                print("room collision detected, waiting additional " + waitBeforeSnapping + " seconds...");
            }
            
        }
        snapReady = true;


    }


    void Start()
    {
        waitBeforeSnapping = numberOfRooms / 2.0f + (maxRoomHeight + maxRoomWidth) / 20.0f;
        //waitBeforeSnapping = 10.0f;

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

        print("Generating " + numberOfRooms + " rooms...");
        for (int i = 1; i <= numberOfRooms; i++)
        {
            //possible addition: add a randomised spread offset and allow user to select the size and shape of the offset?
            rndWidth = Random.Range(minRoomWidth, maxRoomWidth + 1);
            rndHeight = Random.Range(minRoomHeight, maxRoomHeight + 1);
            rndOffset = new Vector2(Random.Range(-spawnSpreadX, spawnSpreadX), Random.Range(-spawnSpreadY, spawnSpreadY));

            rooms.Add(RoomGenScript.CreateRoom(DebugFloorTile,DebugWallTile, UNIT_SIZE, rndWidth,rndHeight,rndOffset));
        }
        print("Rooms generated, waiting " + waitBeforeSnapping + " seconds for physics to resolve...");
        StartCoroutine(WaitForPhysicsSeperation());
        //LOGIC MOVES ON INTO THE UPDATE FUNCTION


    }

    // Update is called once per frame
    void Update()
    {
        if (snapReady)
        {
            snapReady = false;
            print("Timer up, enabling wall tile colliders and snapping...");
            EnableWallTileColliders(rooms);
            SnapToGrid.Run(rooms, UNIT_SIZE);
            print("Preperation Complete, rooms are ready for corridors...");

        }

        //enable only once:
        //  - rooms are spread out with 1 unit tile of space between.
        //  - rooms are snapped to unit grid.
        //  - (consider once pathing implemented) when all wall tiles were converted into floor tiles.
        //EnableWallTileColliders(rooms);
    }
    //----------------------------------
}
