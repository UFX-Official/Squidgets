using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller3D : MonoBehaviour
{
    public enum ControlMethod { MouseInput }
    public ControlMethod controls;

    public LineRenderer line = null;
    public Animator anim = null;

    Rigidbody2D rigid2D;
    float scale = 1.0f;
    public float minScale = 0.45f;
    
    Vector2 direction = Vector2.zero;
    Vector2 mouseUpPos = Vector2.zero;
    Vector2 mouseDownPos = Vector2.zero;
    bool isCharging;
    bool isGrounded;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Controls();

        // Animations
        {
            Vector3 local = transform.localScale;
            transform.localScale = new Vector3(local.x, scale, local.z);
        }

        // Ground Check
        {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, -transform.up, transform.localScale.y);
            if (hit2D.collider != null)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        // LINE DRAW
        {
            if (line.enabled)
            {
                // Track Mouse Down to Mouse Drag
                {
                    Vector3 start = Camera.main.ScreenToWorldPoint(mouseDownPos);
                    Vector3 end = Camera.main.ScreenToWorldPoint(mouseUpPos);

                    start.z = 0;
                    end.z = 0;

                    if (Input.GetMouseButtonDown(1))
                    {
                        Debug.Log(start);
                    }
                    
                    line.SetPosition(0, start);
                    line.SetPosition(1, end);
                }                
            }
        }
    }


    public void Controls()
    {
        switch (controls)
        {
            case ControlMethod.MouseInput:
                
                // Begin Charging Leap
                if (Input.GetMouseButtonDown(0))
                {
                    mouseDownPos = Input.mousePosition;
                    isCharging = true;
                    anim.SetBool("IsCharging", isCharging);
                    line.enabled = true;
                }

                // Handle Charging Scaling
                if (Input.GetMouseButton(0) && isCharging)
                {
                    mouseUpPos = Input.mousePosition;
                    direction = mouseUpPos - mouseDownPos;

                    float charge = direction.magnitude / 500;
                    float result = 1.0f - charge;

                    if (result > minScale)
                        scale = result;
                    else if (result < minScale)
                        scale = minScale;
                }

                // Release the Charging of a leap
                if (Input.GetMouseButtonUp(0) && isCharging)
                {
                    isCharging = false;
                    anim.SetBool("IsCharging", isCharging);
                    line.enabled = false;
                }

                // If grounded, and a charge was released, launch the player
                if (isGrounded && !isCharging && scale < 1.0f)
                {
                    rigid2D.velocity = Vector2.zero;
                    rigid2D.AddForce(-direction.normalized * (5 / scale), ForceMode2D.Impulse);
                    scale = 1.0f;
                }

                break;
        }



    }
}
