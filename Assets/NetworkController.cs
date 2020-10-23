using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    #region Client-Side Variables

    public enum ControlMethod { MouseInput }
    public ControlMethod controls;

    LineRenderer line;
    Rigidbody2D rigid2D;

    Vector2 direction = Vector2.zero;
    Vector2 mouseUpPos = Vector2.zero;
    Vector2 mouseDownPos = Vector2.zero;

    bool isCharging;
    bool isGrounded; // Consider moving to server-sided variable for ability interactions on grounded/non-grounded characters
    
    #endregion

    #region Server-Side Variables

    PhotonView pView = null;

    float scale = 1.0f;
    public float maxScale = 1.0f;
    public float minScale = 0.45f;


    #endregion

    #region MonoBehaviur Callbacks

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        pView = GetComponent<PhotonView>();
        line = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();

        scale = maxScale;
    }
    
    private void Update()
    {
        // Client-Side Controls
        if (pView.IsMine && PhotonNetwork.IsConnected)
        {
            Controls();
        }

        // Animate
        {
            // ANIMATE
            Vector3 local = transform.localScale;
            transform.localScale = new Vector3(local.x, scale, local.y);
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

        // Line Draw
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

    #endregion


    #region Custom Methods

    public void Controls()
    {
        switch(controls)
        {
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
                    rigid2D.AddForce(-direction.normalized * (2.5f / scale), ForceMode2D.Impulse);
                    scale = maxScale;
                }

                break;
        }
    }

    #endregion

}
