using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : Hazard
{
    public override void Trigger(Controller controller)
    {
        if (controller.GetComponent<Entity>() != null)
        {
            controller.GetComponent<Entity>().Kill();
        }
    }
}
