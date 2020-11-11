using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
{
    // #########################################################
    // # ----------------- PLAYER VARIABLES ------------------ #
    // #########################################################

    [SerializeField] private SampleController defaultController = null;
    public Controller activeController;

    // #########################################################
    // -------------------- INITIALIZATION ------------------- #
    // #########################################################

    private void Start()
    {
        if (activeController != null)
        {
            activeController.Possess(this);
        }

        else if (defaultController != null)
        {
            activeController = defaultController.Possess(this);
        }
    }

    // #########################################################
    // # ---------------------- UPDATE ----------------------- #
    // #########################################################

    private void Update()
    {
        if (activeController != null)
            activeController.Tick();        
    }

    public void Possess(Controller controller)
    {
        Debug.Log("Possession Called By MyPlayer");

        // Release control over current controller
        if (activeController != null)
            activeController.Eject();

        // Take possession of the new controller
        activeController = controller;
        activeController.Possess(this);
    }
}
