using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleController : Controller
{
    #region Variables

    public enum ControlMethod { MouseInput, KeyInput, ControllerInput }
    public ControlMethod controls;

    LineRenderer line;

    Rigidbody2D rigid2D;

    float scale = 1.0f;
    public float maxScale = 1.0f;
    public float minScale = 0.45f;
    public float rotationRate = 45;

    Vector2 direction = Vector2.zero;
    Vector2 mouseUpPos = Vector2.zero;
    Vector2 mouseDownPos = Vector2.zero;
    bool mouseDown = false;

    bool isGrounded = false;
    bool isCharging = false;

    bool isSwimming = false;
    
    #endregion

    #region MonoBehaviour Callbacks

    protected override void Awake()
    {
        base.Awake();

        rigid2D = GetComponent<Rigidbody2D>();
        line = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();

        scale = maxScale;
    }

    public void Update()
    {
        if (paused) return;
        
        // ANIMATE
        {
            Vector3 local = transform.localScale;
            transform.localScale = new Vector3(local.x, scale, local.y);

            if (isSwimming)
            {
                transform.up = rigid2D.velocity.normalized;
            }

            else if (!isSwimming && isGrounded)
            {
                transform.up = Vector3.up;
            }
              
        }

        // CHECK GROUNDED     
        {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, -transform.up, transform.localScale.y * 1.25f);
            Debug.DrawLine(transform.position, transform.position - (transform.up * transform.localScale.y * 1.25f));
            if (hit2D.collider != null)
            {
                //Debug.Log("Grounded: (" + hit2D.collider.name + ")");
                if (hit2D.collider.gameObject.tag != "Water")
                {
                    isGrounded = true;
                }
            }
            else
            {
                isGrounded = false;
            }
        }
    }


    #endregion

    #region Custom Methods

    public void Controller()
    {
        switch (controls)
        {
            case ControlMethod.KeyInput:

                #region disabled

                //// MOTION CONTROLS
                //{
                //    // Begin Charging Leap
                //    if (Input.GetKey(KeyCode.Space))
                //    {
                //        if (scale > minScale)
                //            scale -= Time.deltaTime;
                //        else if (scale < minScale)
                //            scale = minScale;

                //        isCharging = true;
                //    }

                //    // Release the Charging of Leap
                //    if (Input.GetKeyUp(KeyCode.Space))
                //    {
                //        isCharging = false;
                //    }

                //    // If grounded and a charge was released, launch the player
                //    if (isGrounded && Input.GetKeyUp(KeyCode.Space))
                //    {
                //        rigid.AddForce(transform.up * (5 / scale), ForceMode2D.Impulse);
                //        scale = maxScale;
                //    }
                //    else if (isGrounded && !isCharging && scale < maxScale)
                //    {
                //        rigid.velocity = Vector3.zero;
                //        rigid.AddForce(transform.up * (5 / scale), ForceMode2D.Impulse);
                //        scale = maxScale;
                //    }
                //}

                //// ROTATION CONTROLS
                //{
                //    if (Input.GetKey(KeyCode.LeftArrow))
                //    {
                //        // Rotate Counter Clock-Wise 
                //        float rotationForce = rotationRate * Time.deltaTime;
                //        transform.Rotate(0, 0, rotationForce);
                //    }

                //    if (Input.GetKey(KeyCode.RightArrow))
                //    {
                //        // Rotate Clock-Wise
                //        float rotationForce = -rotationRate * Time.deltaTime;
                //        transform.Rotate(0, 0, rotationForce);
                //    }
                //}

                #endregion


                break;

            case ControlMethod.ControllerInput:

                break;

            case ControlMethod.MouseInput:

                // Begin Charging Leap
                if (Input.GetMouseButtonDown(0))
                {
                    mouseDownPos = Input.mousePosition;
                    isCharging = true;
                    line.enabled = true;
                }

                // Handle Charging Scaling
                if (Input.GetMouseButton(0) && isCharging)
                {
                    mouseUpPos = Input.mousePosition;
                    direction = mouseUpPos - mouseDownPos;
                    
                    float charge = direction.magnitude / 500;
                    float result = maxScale - charge;

                    if (result > minScale)
                        scale = result;
                    else if (result < minScale)
                        scale = minScale;
                }
                
                // Release the Charging of a leap
                if (Input.GetMouseButtonUp(0) && isCharging)
                {
                    isCharging = false;
                    line.enabled = false;
                }

                // If Grounded, and a charge was released, launch the player
                if (isGrounded && !isCharging && scale < maxScale)
                {
                    rigid2D.velocity = Vector2.zero;
                    Launch(-direction.normalized, (2.5f / scale));
                    scale = maxScale;
                }

                if (isSwimming && !isCharging && scale < maxScale)
                {
                    Launch(-direction.normalized, (1.25f / scale));
                    scale = maxScale;
                }

                // Draw Line
                if (line.enabled)
                {
                    Vector3 start = Camera.main.ScreenToWorldPoint(mouseDownPos);
                    Vector3 end = Camera.main.ScreenToWorldPoint(mouseUpPos);

                    start.z = 0;
                    end.z = 0;

                    line.SetPosition(0, start);
                    line.SetPosition(1, end);
                }

                break;                
        }
    }

    public void Launch(Vector3 direction, float force)
    {
        rigid2D.AddForce(direction * force, ForceMode2D.Impulse);
        Debug.Log("Launching with force (" + force + ") in direction (" + direction + ")");
    }

    #endregion

    #region Collisions

    public void OnTriggerEnter2D(Collider2D collision)
    {

        // On collision with water, break the 
        if (collision.gameObject.tag == "Water")
        {
            // Break the players speed when they enter water
            rigid2D.velocity = rigid2D.velocity / 2.0f;

            // Set linear drag to 1.0f.  This simulates the player having to push harder to get through water than air.
            rigid2D.drag = 1.0f;
        }

        if (collision.gameObject.tag == "Hazard")
        {
            collision.gameObject.GetComponent<Hazard>().Trigger(this);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (paused) return;

        if (collision.gameObject.GetComponent<I_Interactable>() != null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                collision.gameObject.GetComponent<I_Interactable>().Interact(GetPlayer());
            }
        }

        if (collision.gameObject.tag == "Water")
        {
            // While in water, float upwards towards the surface.
            rigid2D.AddForce(Vector2.up * 12.0f, ForceMode2D.Force);
            isSwimming = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            // Upon leaving the water, set isSwimming to false
            isSwimming = false;

            // Set linear drag back to zero.
            rigid2D.drag = 0.0f;
        }
    }
    
    #endregion

    #region Controller Inherited Methods

    public override void Tick()
    {
        if (owner == null) return;
        
        Controller();
    }

    public override Controller Possess(MyPlayer player)
    {
        return base.Possess(player);
    }

    public override void Eject()
    {
        base.Eject();
    }

    #endregion




}
