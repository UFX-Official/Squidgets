using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListingManager : MonoBehaviour
{
    public Text lobbyNameText = null;
    public Text lobbyGameModeText = null;
    public Text lobbyMapText = null;
    public Text lobbyPlayerCountText = null;


    public void Initialize(LobbyListing l)
    {
        lobbyNameText.text = l.lobbyName;
        lobbyGameModeText.text = l.lobbyGameMode.ToString();
        lobbyMapText.text = l.lobbyMap.ToString();
        lobbyPlayerCountText.text = l.currentPlayerCount + " / " + l.maxPlayerCount;
    }

}
