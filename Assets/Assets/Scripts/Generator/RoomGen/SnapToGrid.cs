using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    public void Run(GameObject item,float unit)
    {

        try
        {
            item.GetComponent<Rigidbody2D>().simulated = false;
        }
        catch
        {
            //no rigidbody to re-enable
        }
        

        Transform Tr = item.GetComponent<Transform>();
        Tr.position = new Vector3((((int)Tr.position.x / unit) * unit),
                                        (((int)Tr.position.y / unit) * unit), Tr.position.z);
        item.GetComponent<Transform>().position = Tr.position;
        
        try
        {
            if (item.transform.name != "Room")
            {
                item.GetComponent<Rigidbody2D>().simulated = true;
            }
                
        }
        catch
        {
            //no rigidbody to re-enable
        }

    }

    public void Run(List<GameObject> items, float unit)
    {
        foreach (GameObject item in items)
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
