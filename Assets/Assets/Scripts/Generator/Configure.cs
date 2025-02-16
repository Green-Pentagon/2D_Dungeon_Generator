//TO DO:
//- Add wall tiles to rooms & ensure they are given a 2d box collider
//- Add functionality that will spread the rooms out 1 unit tile apart & snap them to a unit determined grid
//- RE-ENABLE ALL WALL TILE BOX COLLIDERS AFTER ROOMS ARE SEPERATED
//- Add the ability to select how you want to spread the rooms out & the size of the spread
//- Corridor functionality
//- wall-tile to corridor on corridor touch functionality
//- clean up dead ends (configurable?)
//- surround corridor tiles with walls functionality



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configure : MonoBehaviour
{
    //Class attributes
    //----------------------------------
    private GenerateRoom RoomGenScript;
    public float UNIT_SIZE = 1.0f;
    public Sprite DebugFloorTile;
    public Sprite DebugWallTile;

    //IF YOU CHANGE THESE VALUES, ALSO CHANGE THE VALUES OFR THE PUBLIC VARIABLES BELOW ACCORDINGLY.
    private int[] roomWidthRange = { 3, 100 };
    private int[] roomHeightRange = { 3, 100 };
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
    [Range(0, 50)]
    public int spawnSpreadX;
    [Range(0, 50)]
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
    
    
    
    void Start()
    {
        RoomGenScript = GetComponentInParent<GenerateRoom>();

        if (randomiseSeedOnStart)
        {
            seed = (int)System.DateTime.Now.Ticks;
        }
        Random.InitState(seed);
        print("Generated Seed: " + seed + "\nRandom number from 1 to 100: " + Random.Range(1,100));

        rooms = new List<GameObject>();
        int rndWidth;
        int rndHeight;
        Vector2 rndOffset;

        for (int i = 1; i <= numberOfRooms; i++)
        {
            //possible addition: add a randomised spread offset and allow user to select the size and shape of the offset?
            rndWidth = Random.Range(minRoomWidth, maxRoomWidth + 1);
            rndHeight = Random.Range(minRoomHeight, maxRoomHeight + 1);
            rndOffset = new Vector2(Random.Range(-spawnSpreadX, spawnSpreadX), Random.Range(-spawnSpreadY, spawnSpreadY));

            rooms.Add(RoomGenScript.CreateRoom(DebugFloorTile,DebugWallTile, UNIT_SIZE, rndWidth,rndHeight,rndOffset));
        }

        //enable only once:
        //  - rooms are spread out with 1 unit tile of space between.
        //  - rooms are snapped to unit grid.
        //  - (consider once pathing implemented) when all wall tiles were converted into floor tiles.
        //EnableWallTileColliders(rooms);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //----------------------------------
}
