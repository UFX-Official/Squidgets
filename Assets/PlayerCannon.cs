using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCannon : Controller, I_Interactable
{
    [SerializeField] Controller loaded = null;
    [SerializeField] Transform pivot = null;
    [SerializeField] Transform shotOrigin = null;

    [SerializeField] Vector2 rotationClamp = Vector2.zero;
    [SerializeField] float rotationSpeed = 1.0f;
    [SerializeField] float shotForce = 10.0f;

    private Vector3 cannonRotation = Vector3.zero;

    #region Controls

    private void RotateCannon()
    {
        // Rotate CCW
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (cannonRotation.z > rotationClamp.x)
                Mathf.Clamp(cannonRotation.z, rotationClamp.y, rotationClamp.x);

            else
                cannonRotation.z += Time.deltaTime * rotationSpeed;
           
            pivot.transform.eulerAngles = cannonRotation;

        }

        // Rotate CW
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (cannonRotation.z < rotationClamp.y)
                Mathf.Clamp(cannonRotation.z, rotationClamp.y, rotationClamp.x);

            else
                cannonRotation.z -= Time.deltaTime * rotationSpeed;

            pivot.transform.eulerAngles = cannonRotation;
        }        
    }

    private void ShootCannon()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            loaded.gameObject.transform.position = shotOrigin.position;
            loaded.gameObject.SetActive(true);            
            GetPlayer().Possess(loaded);

            //loaded.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //loaded.GetComponent<Rigidbody2D>().AddForce(shotOrigin.forward * shotForce, ForceMode2D.Impulse);
            loaded.GetComponent<SampleController>().Launch(shotOrigin.right, shotForce);

            loaded = null;
        }
    }

    #endregion

    #region I_Interactable Implemented Methods

    public void Interact(MyPlayer player)
    {
        Debug.Log("Interacting with Cannon");

        loaded = player.activeController;
        loaded.gameObject.SetActive(false);

        player.Possess(this);
    }

    #endregion

    #region Controller Inherited Methods

    public override void Tick()
    {
        if (owner == null) return;

        RotateCannon();
        ShootCannon();

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
