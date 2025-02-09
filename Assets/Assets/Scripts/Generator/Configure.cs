using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configure : MonoBehaviour
{
    public const float UNIT_SIZE = 1.0f;

    public int numRooms = 1;
    public int[] roomWidthRange = {1,5};
    public int[] roomHeightRange = {1,5};
    
    
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
