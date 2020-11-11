using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterConsole : MonoBehaviour, I_Interactable
{
    private TeleporterPrime prime;
    
    public void LinkToSystem(TeleporterPrime _tp)
    {
        prime = _tp;
    }
    
    public void Interact(MyPlayer player)
    {
        prime.Interact(player);
    }
}
