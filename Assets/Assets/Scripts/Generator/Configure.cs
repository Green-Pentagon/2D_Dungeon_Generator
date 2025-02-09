using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configure : MonoBehaviour
{

    //----------------------------------
    public const float UNIT_SIZE = 1.0f;

    //IF YOU CHANGE THESE VALUES, ALSO CHANGE THE VALUES OFR THE PUBLIC VARIABLES BELOW ACCORDINGLY.
    private int[] roomWidthRange = { 1, 100 };
    private int[] roomHeightRange = { 1, 100 };
    //private int[] roomCountRange = { 1, 6 };
    //----------------------------------


    //IN-EDITOR SLIDERS
    //----------------------------------
    //room count sliders
    [Range(1, 6)]
    //[Min(1), Tooltip("WARNING: High room counts may impact performance!")]
    public int numRooms = 1;

    //room width sliders
    [Range(1, 100)]
    public int minimumWidth;
    [Range(1, 100)]
    public int maximumWidth;

    //room height sliders
    [Range(1, 100)]
    public int minimumHeight;
    [Range(1, 100)]
    public int maximumHeight;
    //----------------------------------

    //IN-EDITOR SLIDER VALIDATION
    //----------------------------------
    [ExecuteInEditMode]
    void OnValidate()
    {

        if (minimumWidth > maximumWidth && maximumWidth != roomWidthRange[1])
        {
            minimumWidth++;
        }

        if (minimumHeight > maximumHeight && maximumHeight != roomHeightRange[1])
        {
            maximumHeight++;
        }
    }
    //----------------------------------

    private List<Object> rooms;
    void Start()
    {
        rooms = new List<Object>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
