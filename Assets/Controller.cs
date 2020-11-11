using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // #########################################################
    // # --------------- CONTROLLER VARIABLES ---------------- #
    // #########################################################

    // Active Owner
    [SerializeField] protected MyPlayer owner;
    public bool paused = false;
    
    // #########################################################
    // -------------------- INITIALIZATION ------------------- #
    // #########################################################

    protected virtual void Awake()
    {
        // Disable components if not possessed
        if (owner == null)
        {
            Eject();
        }
    }

    // #########################################################
    // # ---------------------- UPDATE ----------------------- #
    // #########################################################

    public virtual void Tick() {}

    // #########################################################
    // # --------------------- CONTROLS ---------------------- #
    // #########################################################

    // Used for pausing the controllers functionality
    public void SetState(bool state)
    {
        paused = state;
    }

    // Take Control and Initialize all components of the controller
    public virtual Controller Possess(MyPlayer player)
    {
        Debug.Log(name + ": Possessing");
        owner = player;
        
        return this;
    }

    // Disable all the components of the controller and put it to sleep
    public virtual void Eject()
    {
        Debug.Log(name + " : Ejecting");
        owner = null;
    }

    public MyPlayer GetPlayer() { return owner; }
}
