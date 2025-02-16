//TO DO:
//- Add wall tiles to rooms & ensure they are given a 2d box collider
//- Add functionality that will spread the rooms out 1 unit tile apart & snap them to a unit determined grid
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

    //IF YOU CHANGE THESE VALUES, ALSO CHANGE THE VALUES OFR THE PUBLIC VARIABLES BELOW ACCORDINGLY.
    private int[] roomWidthRange = { 3, 100 };
    private int[] roomHeightRange = { 3, 100 };
    //private int[] roomCountRange = { 1, 6 };

    private List<Object> rooms;
    //----------------------------------
    //IN-EDITOR SLIDERS
    //----------------------------------

    //Random Seed Configuration
    [Header("Seed Settings", order = 0)]
    [Space(2, order = 1)]
    public int seed;
    public bool randomiseSeedOnStart = true;


    [Header("Room Settings", order = 2)]
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
    void Start()
    {
        RoomGenScript = GetComponentInParent<GenerateRoom>();

        if (randomiseSeedOnStart)
        {
            seed = (int)System.DateTime.Now.Ticks;
        }
        Random.InitState(seed);
        print("Generated Seed: " + seed + "\nRandom number from 1 to 100: " + Random.Range(1,100));

        rooms = new List<Object>();

        for (int i = 1; i <= numberOfRooms; i++)
        {
            //possible addition: add a randomised spread offset and allow user to select the size and shape of the offset?
            rooms.Add(RoomGenScript.CreateRoom(DebugFloorTile, UNIT_SIZE, Random.Range(minRoomWidth, maxRoomWidth + 1), Random.Range(minRoomHeight, maxRoomHeight + 1)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //----------------------------------
}
