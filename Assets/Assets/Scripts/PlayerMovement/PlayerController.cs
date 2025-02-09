using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    
    float moveX = 0.0f;
    float moveY = 0.0f;
    float velocityMult = 3.0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        moveX = Input.GetAxis("Horizontal") * velocityMult;
        moveY = Input.GetAxis("Vertical") * velocityMult;
        rb.velocity = new Vector2 (moveX, moveY);
    }
}
