using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    public void Run(Room item,float unit)
    {
        Transform Tr = item.GetRoom().GetComponent<Transform>();
        Tr.position = new Vector3((((int)Tr.position.x / unit) * unit),
                                        (((int)Tr.position.y / unit) * unit), Tr.position.z);
        item.GetRoom().GetComponent<Transform>().position = Tr.position;
        
    }

    public void Run(List<Room> items, float unit)
    {
        foreach (Room item in items)
        {
            Run(item, unit);
        }
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
