using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configure : MonoBehaviour
{
    //Class attributes
    //----------------------------------
    public const float UNIT_SIZE = 1.0f;

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
    [Range(1, 6)]
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
        if (randomiseSeedOnStart)
        {
            seed = (int)System.DateTime.Now.Ticks;
        }
        Random.InitState(seed);
        print("Generated Seed: " + seed + "\nRandom number from 1 to 100: " + Random.Range(1,100));

        rooms = new List<Object>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //----------------------------------
}
