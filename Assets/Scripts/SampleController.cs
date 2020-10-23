using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleController : MonoBehaviour
{
    #region Variables

    public enum ControlMethod { MouseInput, KeyInput, ControllerInput }
    public ControlMethod controls;

    LineRenderer line;

    Rigidbody2D rigid;

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

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        line = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();

        scale = maxScale;
    }

    public void Update()
    {
        Controller();

        // ANIMATE
        {
            Vector3 local = transform.localScale;
            transform.localScale = new Vector3(local.x, scale, local.y);
        }

        // CHECK GROUNDED     
        {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, -transform.up, transform.localScale.y * 1.25f);
            Debug.DrawLine(transform.position, transform.position - (transform.up * transform.localScale.y * 1.25f));
            if (hit2D.collider != null)
            {
                //Debug.Log("Grounded: (" + hit2D.collider.name + ")");
                isGrounded = true;
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

                // MOTION CONTROLS
                {
                    // Begin Charging Leap
                    if (Input.GetKey(KeyCode.Space))
                    {
                        if (scale > minScale)
                            scale -= Time.deltaTime;
                        else if (scale < minScale)
                            scale = minScale;

                        isCharging = true;
                    }

                    // Release the Charging of Leap
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        isCharging = false;
                    }

                    // If grounded and a charge was released, launch the player
                    if (isGrounded && Input.GetKeyUp(KeyCode.Space))
                    {
                        rigid.AddForce(transform.up * (5 / scale), ForceMode2D.Impulse);
                        scale = maxScale;
                    }
                    else if (isGrounded && !isCharging && scale < maxScale)
                    {
                        rigid.velocity = Vector3.zero;
                        rigid.AddForce(transform.up * (5 / scale), ForceMode2D.Impulse);
                        scale = maxScale;
                    }
                }

                // ROTATION CONTROLS
                {
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        // Rotate Counter Clock-Wise 
                        float rotationForce = rotationRate * Time.deltaTime;
                        transform.Rotate(0, 0, rotationForce);
                    }

                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        // Rotate Clock-Wise
                        float rotationForce = -rotationRate * Time.deltaTime;
                        transform.Rotate(0, 0, rotationForce);
                    }
                }

                break;

            case ControlMethod.ControllerInput:

                break;

            case ControlMethod.MouseInput:

                // MOTION CONTROLS
                {
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

                        // Debug.DrawLine(mouseDownPos, mouseUpPos);

                        float charge = direction.magnitude / 500;
                        //Debug.Log(charge);

                        float result = maxScale - charge;

                        if (result > minScale)
                            scale = result;
                        else if (result < minScale)
                            scale = minScale;

                        isCharging = true;
                    }

                    // Release the Charging of a leap
                    if (Input.GetMouseButtonUp(0) && isCharging)
                    {
                        isCharging = false;
                        line.enabled = false;
                    }

                    // If grounded and a charge was released, launch the player
                    if (isGrounded && Input.GetKeyUp(KeyCode.Space))
                    {
                        rigid.AddForce(transform.up * (2.5f / scale), ForceMode2D.Impulse);
                        scale = maxScale;
                    }
                    else if (isGrounded && !isCharging && scale < maxScale)
                    {
                        rigid.velocity = Vector3.zero;
                        rigid.AddForce(transform.up * (2.5f / scale), ForceMode2D.Impulse);
                        scale = maxScale;
                    }


                }

                // ROTATION CONTROLS
                {
                    if (isCharging)
                    {
                        Vector3 mousePos = Input.mousePosition;
                        mousePos.z = -10; // Distance between camera and object

                        //Vector3 objPos = Camera.main.WorldToScreenPoint(transform.position);
                        Vector3 objPos = mouseDownPos;
                        mousePos.x = mousePos.x - objPos.x;
                        mousePos.y = mousePos.y - objPos.y;

                        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
                        angle = angle + 90;

                        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
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

                break;
        }
    }

    #endregion





}
