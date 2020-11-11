using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterPrime : MonoBehaviour, I_Interactable
{
    public Controller user = null;
    [SerializeField] Canvas teleporterGUI = null;
    [SerializeField] List<TeleporterConsole> linkedConsoles = new List<TeleporterConsole>();

    private void Awake()
    {
        if (linkedConsoles.Count > 0)
        {
            foreach (TeleporterConsole tc in linkedConsoles)
            {
                tc.LinkToSystem(this);
            }
        }
        
    }

    public void Interact(MyPlayer player)
    {
        if (user == null)
        {
            teleporterGUI.enabled = true;
            user = player.activeController;
            user.SetState(true);
            user.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }        
    }

    public void CloseCanvas()
    {
        teleporterGUI.enabled = false;
        user.SetState(false);
        user = null;
    }

    public void TeleportTo(int index)
    {
        user.gameObject.transform.position = linkedConsoles[index].transform.position;
    }
}
