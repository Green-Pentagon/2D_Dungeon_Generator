using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configure : MonoBehaviour
{

    //----------------------------------
    public const float UNIT_SIZE = 1.0f;

    //IF YOU CHANGE THESE VALUES, ALSO CHANGE THE VALUES OFR THE PUBLIC VARIABLES BELOW ACCORDINGLY.
    private int[] roomWidthRange = { 3, 100 };
    private int[] roomHeightRange = { 3, 100 };
    //private int[] roomCountRange = { 1, 6 };
    //----------------------------------


    //IN-EDITOR SLIDERS
    //----------------------------------

    [Header("Room Settings", order = 0)]
    [Space(2, order = 1)]


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

    
    [Min(1)]
    public int seed;
    public bool randomiseSeedOnStart = true;
    
    
    
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

    private List<Object> rooms;

    void Start()
    {
        if (randomiseSeedOnStart)
        {
            seed = (int)System.DateTime.Now.Ticks;
        }
        Random.InitState(seed);
        print("Called in Start " + seed);

        rooms = new List<Object>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
