using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnableEntity : Entity
{
    public float respawnTimer = 3.0f;

    public override void Kill()
    {
        StartCoroutine(Respawn());

        if (GetComponent<Controller>() != null)
        {
            GetComponent<Controller>().paused = true;
        }
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTimer);
        
        GetComponent<Controller>().paused = false;
        transform.position = NetworkManager.GetRandomSpawnPoint().position;
    }
}
