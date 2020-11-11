using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Client-Side Variables

    [SerializeField] 
    MyPlayer player = null;

    [SerializeField]
    GameObject playerPrefab = null;

    [SerializeField]
    static GameObject[] spawnPoint = null;


    [SerializeField] 
    Canvas victoryCanvas = null;

    [SerializeField]
    Text playerVictoryDisplay = null;

    [SerializeField]
    bool DebugSinglePlayer = false;

    [SerializeField]
    VictoryCollider victoryCondition = null;


    #endregion

    #region Server-Side Variables

    bool gameComplete = false;

    #endregion


    #region MonoBehaviour Callbacks

    private void Awake()
    {
        spawnPoint  = GameObject.FindGameObjectsWithTag("Respawn");        
    }

    private void Start()
    {
        Debug.Log("Starting...");
        gameComplete = false;
        victoryCanvas.enabled = false;


        if (DebugSinglePlayer)
        {
            int index = Random.Range(0, spawnPoint.Length - 1);

            GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefabs/SPController"), spawnPoint[index].transform.position, Quaternion.identity);
            player.Possess(playerObj.GetComponent<Controller>());
            Camera.main.GetComponent<CameraFollow>().SetTarget(playerObj.transform);            
        }

        else if (PhotonNetwork.IsConnected)
        {
            int index = Random.Range(0, spawnPoint.Length - 1);

            GameObject playerObj = PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, spawnPoint[index].transform.position, Quaternion.identity, 0);
            Camera.main.GetComponent<CameraFollow>().SetTarget(playerObj.transform);

        }
    }

    private void Update()
    {

        if (victoryCondition.victory)
        {
            // Multiplayer Trigger
            if (PhotonNetwork.IsConnected)
            {

                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("Win", RpcTarget.All, victoryCondition.winner);
            }

            // Single Player Trigger
            else
            {
                Win(victoryCondition.winner);
            }
        }
        

        
    }

    #endregion

    #region Custom Methods

    [PunRPC]
    public void Win(string playerName)
    {
        victoryCanvas.enabled = true;
        playerVictoryDisplay.text = playerName + " has won!";

        gameComplete = true;
    }

    public static Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, spawnPoint.Length - 1);
        return spawnPoint[index].transform;
    }

    #endregion





}
