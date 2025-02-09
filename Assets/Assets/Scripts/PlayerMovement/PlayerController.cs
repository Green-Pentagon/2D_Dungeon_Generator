using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public KeyCode jumpKey = KeyCode.Space;

    
    float moveX = 0.0f;
    float velocityMult = 1.0f;
    bool jumpKeyHeld = false;
    bool jumped = false;
    bool grounded = false;
    Vector2 boxExtents;
    Vector2 jumpForce = new Vector2(0.0f, 10.0f);


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxExtents = GetComponent<CapsuleCollider2D>().bounds.extents;// get the extent of the collision box
    }

    private void FixedUpdate()
    {
        // check if we are on the ground
        Vector2 bottom = new Vector2(transform.position.x, transform.position.y - boxExtents.y);
        Vector2 hitBoxSize = new Vector2(boxExtents.x * 2.0f, 0.05f);
        RaycastHit2D result = Physics2D.BoxCast(bottom, hitBoxSize, 0.0f, new Vector3(0.0f, -1.0f), 0.0f, 1 << LayerMask.NameToLayer("Ground"));
        grounded = result.collider != null && result.normal.y > 0.9f;
    }

    // Update is called once per frame
    void Update()
    {
        jumpKeyHeld = Input.GetKeyDown(jumpKey);
        
        if (jumpKeyHeld)
        {
            jumped = true;
            rb.AddRelativeForce(jumpForce,ForceMode2D.Force);
        }
        else if (grounded) {
            jumped = false;
        }
        
        moveX = Input.GetAxis("Horizontal") * velocityMult;
        rb.velocity = new Vector2 (moveX, rb.velocity.y);
    }
}
