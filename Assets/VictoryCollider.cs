using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCollider : MonoBehaviour
{
    public string winner = "";
    public bool victory = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        winner = other.name;
        victory = true;
    }
}
