using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    #region Private Fields

    string gameVersion = "v0.0.3";

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("LobbyManager: We are now connected to the " + PhotonNetwork.CloudRegion + " server!");
        

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("LobbyManager: OnDisconnected() was called by PUN with reasion {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("LobbyManager: OnJoinRandomFailed() was called by PUN. No Random room available");        
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("LobbyManager: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }

    #endregion

    #region Public Methods

    public void Connect(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.JoinRandomRoom();
            PhotonNetwork.JoinRoom(roomName);
        }

        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    #endregion
}
