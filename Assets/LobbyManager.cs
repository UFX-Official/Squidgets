using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region Debug
    
    [SerializeField]
    private Text playerNameDisplay = null;

    [SerializeField]
    private Text currentRoomNameDisplay = null;

    [SerializeField] 
    private GameObject roomCreationPanel = null;

    [SerializeField] 
    private GameObject lobbyListingPanel = null;

    [SerializeField] 
    private GameObject currentRoomPanel = null;

    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        // Create Test Lobby's
        //rooms.Add(new LobbyListing() { settings = new RoomSettings("Ultimate Squish Masters", GameMode.FreeRoam, Map.Default, 6) });
        //rooms.Add(new LobbyListing() { settings = new RoomSettings("Midget Meisters 2020", GameMode.FreeRoam, Map.Default, 10) });
        //rooms.Add(new LobbyListing() { settings = new RoomSettings("Tinkletot's Tater Tasting", GameMode.FreeRoam, Map.Default, 3) });

        // Display Lobby Listings
        //UpdateLobbyListings();
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        roomListings = new List<RoomInfo>(); // See [r]Lobby Management

        if (PlayerPrefs.HasKey("NickName")) // Check to see if the a nickname is stored in player prefs
        {
            if (PlayerPrefs.GetString("NickName") == "") // If the stored nickname is empty, generate one randomly
            {
                PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
            }

            else // If the stored nickname is not empty, load it.
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
            }
        }

        else // If there is no nickname stored in player prefs, generate one randomly.
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
        }

        playerNameDisplay.text = "Welcome " + PhotonNetwork.NickName;
    }

    #endregion

    #region Player Management

    public void PlayerNameUpdate(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("NickName", nameInput);
        playerNameDisplay.text = nameInput;
    }

    #endregion

    #region Login Page

    public void JoinLobbyOnClick()
    {
        // Close Login Panel
        // Open Lobby Panel
        PhotonNetwork.JoinLobby();
    }

    public void Logout()
    {
        // Close Lobby Panel
        // Open Login Panel
        PhotonNetwork.LeaveLobby();
    }

    #endregion

    #region Lobby Management

    [Header("Lobby Listings Components")]
    public Transform roomListParent = null;
    public GameObject noLobbyText = null;
    
    private List<RoomInfo> roomListings;
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count <= 0)
        {
            noLobbyText.SetActive(true);
            return;
        }

        else
        {
            if (noLobbyText.activeInHierarchy)
            {
                noLobbyText.SetActive(false);
            }

            int index;
            foreach (RoomInfo room in roomList)
            {
                if (roomListings != null)
                {
                    index = roomListings.FindIndex(ByName(room.Name));
                }
                else
                {
                    index = -1;
                }

                if (index != -1)
                {
                    roomListings.RemoveAt(index);
                    Destroy(roomListParent.GetChild(index).gameObject);
                }

                if (room.PlayerCount > 0)
                {
                    roomListings.Add(room);
                    ListRoom(room);
                }
            }
        }        
    }

    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    private void ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject listing = Instantiate(Resources.Load("UI/RoomListing"), roomListParent) as GameObject;
            RoomListingButton button = listing.GetComponent<RoomListingButton>();

            string modeInput = room.CustomProperties["Mode"].ToString();
            string mapInput = room.CustomProperties["Map"].ToString();
            button.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount, modeInput, mapInput);
        }
    }

    //private void UpdateLobbyListings()
    //{
    //    ClearLobbyListings();

    //    // If there's no active listings, display a notice and back out.
    //    if (rooms.Count == 0)
    //    {
    //        if (!noLobbyText.activeInHierarchy)
    //            noLobbyText.SetActive(true);

    //        return;
    //    }

    //    // If an active listing is found, hide the no listing notice
    //    if (noLobbyText.activeInHierarchy)
    //        noLobbyText.SetActive(false);

    //    // Loop through the listings and generate interactable buttons for each one.
    //    for (int i = 0; i < rooms.Count; i++)
    //    {
    //        GameObject listing = Instantiate(Resources.Load("UI/LobbyListing"), lobbyListParent) as GameObject;
    //        LobbyListingManager lm = listing.GetComponent<LobbyListingManager>();
    //        lm.Initialize(rooms[i]);

    //        lobbyListings.Add(listing);
    //    }

    //}

    //private void ClearLobbyListings()
    //{
    //    for (int i = 0; i < lobbyListings.Count; i++)
    //    {
    //        Destroy(lobbyListings[i]);
    //    }

    //    lobbyListings.RemoveAll(delegate (GameObject o) { return o == null; });
    //}


    #endregion

    #region Room Creation Manager

    public const string MAP_PROP_KEY = "MAP";
    public const string MODE_PROP_KEY = "MODE";

    [Header("Room Creation Components")]
    public InputField roomNameInput = null;
    public Dropdown roomGameModeInput = null;
    public Dropdown roomMapInput = null;
    public Dropdown roomMaxPlayerInput = null;

    public void CreateRoom()
    {
        string name = roomNameInput.text;
        GameMode.Mode mode = GameMode.GetModeByID(roomGameModeInput.value);
        GameMap.Map map = GameMap.GetMapByID(roomMapInput.value);
        uint maxPlayers = (uint)roomMaxPlayerInput.value + 1;

        RoomOptions options = new RoomOptions() {
            IsVisible = true,
            IsOpen = true,
            EmptyRoomTtl = 6000,
            MaxPlayers = System.Convert.ToByte(maxPlayers),
            CustomRoomPropertiesForLobby = new string[] { MODE_PROP_KEY, MAP_PROP_KEY },
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { MODE_PROP_KEY, mode }, { MAP_PROP_KEY, map } }
        };

        PhotonNetwork.CreateRoom(name, options);
    }

    public override void OnCreatedRoom()
    {
        roomCreationPanel.SetActive(false);
        currentRoomPanel.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Room Creation failed with error code {0} and error message {1}", returnCode, message);
    }

     

    #endregion

    #region RoomController

    [Header("Room Info Panel")]
    public InputField roomNameChangeInput = null;
    public Dropdown roomModeChangeInput = null;
    public Dropdown roomMapChangeInput = null;
    public Dropdown roomCountChangeInput = null;

    public Text roomNameDisplay = null;
    public Text roomModeDisplay = null;
    public Text roomMapDisplay = null;
    public Text roomCountDisplay = null;



    [SerializeField] 
    private int multiPlayerSceneIndex = 0;

    [SerializeField]
    private Transform roomPlayerListParent = null;

    // Clears the list of players within the room.
    private void ClearPlayerListings()
    {
        for (int i = roomPlayerListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(roomPlayerListParent.GetChild(i).gameObject);
        }
    }

    // Generates a list of players in the room.
    private void ListPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject listing = Instantiate(Resources.Load("UI/PlayerListing"), roomPlayerListParent) as GameObject;
            Text nameDisplay = listing.transform.GetChild(0).GetComponent<Text>();
            nameDisplay.text = player.NickName;
        }
    }

    public override void OnJoinedRoom()
    {
        // Disable Lobby Panel
        // Enable Room Panel
        currentRoomNameDisplay.text = "You are currently in the room: " + PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            // Enable things that only the owner of the room can see
            /// i.e. Room Settings
        }
        else
        {
            // Enable things that only room members can see or hide things that they shouldnt see
        }

        // Refresh the Player List
        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnLeftRoom()
    {
        currentRoomNameDisplay.text = "You are currently not in a room.";
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Refresh the Player List
        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Refresh the Player List
        ClearPlayerListings();
        ListPlayers();

        if (PhotonNetwork.IsMasterClient)
        {
            // Enable things that only the owner can see
            /// For if a room member becomes the room master because the master left the room.
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Hide the game from the room listings 
            PhotonNetwork.CurrentRoom.IsOpen = false; // Comment this out if you want players to join mid game.

            // Load the desired level via scene index
            PhotonNetwork.LoadLevel(multiPlayerSceneIndex); 
        }
    }

    
    IEnumerator RejoinLobby()
    {
        /// Used to circumvent an issue where when the room master leaves the room, the lobby listings
        /// do not refresh for some reason.

        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinLobby();
    }

    public void BackOnClick()
    {
        // Enable Lobby Panel
        // Disable Room Panel
        currentRoomPanel.SetActive(false);

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        StartCoroutine(RejoinLobby()); // To Delay Rejoining the Lobby
    }


    public void ToggleRoomNameChange(bool state)
    {
        // if (!roomOwner) return

        if (state)
        {
            roomNameChangeInput.gameObject.SetActive(true);
            roomNameDisplay.gameObject.SetActive(false);
        }

        else
        {
            roomNameChangeInput.gameObject.SetActive(false);
            roomNameDisplay.gameObject.SetActive(true);
        }
    }

    public void SetRoomName()
    {
        // if (!roomOwner) return
        roomNameDisplay.text = roomNameChangeInput.text;
        ToggleRoomNameChange(false);
    }

    public void ToggleRoomModeChange(bool state)
    {
        if (state)
        {
            roomModeChangeInput.gameObject.SetActive(true);
            roomModeDisplay.gameObject.SetActive(false);

            roomModeChangeInput.Show();
        }

        else
        {
            roomModeChangeInput.gameObject.SetActive(false);
            roomModeDisplay.gameObject.SetActive(true);
        }
    }

    public void SetRoomMode()
    {
        roomModeDisplay.text = "Gamemode: " + GameMode.GetNameByID(roomModeChangeInput.value);
        ToggleRoomModeChange(false);
    }

    public void ToggleRoomMapChange(bool state)
    {
        if (state)
        {
            roomMapChangeInput.gameObject.SetActive(true);
            roomMapDisplay.gameObject.SetActive(false);

            roomMapChangeInput.Show();
        }

        else
        {
            roomMapChangeInput.gameObject.SetActive(false);
            roomMapDisplay.gameObject.SetActive(true);
        }
    }

    public void SetRoomMap()
    {
        roomMapDisplay.text = "Map: " + GameMap.GetNameByID(roomMapChangeInput.value);
        ToggleRoomMapChange(false);
    }

    public void ToggleRoomCountChange(bool state)
    {
        if (state)
        {
            roomCountChangeInput.gameObject.SetActive(true);
            roomCountDisplay.gameObject.SetActive(false);

            roomCountChangeInput.Show();
        }

        else
        {
            roomCountChangeInput.gameObject.SetActive(false);
            roomCountDisplay.gameObject.SetActive(true);
        }
    }

    public void SetRoomCount()
    {
        roomCountDisplay.text = "Player Count: 0 / " + (roomCountChangeInput.value + 1).ToString();
        ToggleRoomCountChange(false);
    }


    #endregion

    #region Public Methods



    //public void CreateRoom()
    //{
    //    string name = roomNameInput.text;
    //    GameMode.Mode mode = (GameMode.Mode)roomGameModeInput.value;
    //    GameMap.Map map = (GameMap.Map)roomMapInput.value;
    //    uint maxPlayers = (uint)roomMaxPlayerInput.value + 1;
        
    //    //RoomSettings _settings = new RoomSettings(name, mode, map, maxPlayers);

    //    //LobbyListing listing = new LobbyListing() { settings = _settings };
    //    //rooms.Add(listing);

        

    //    RoomOptions roomOptions = new RoomOptions();    

    //    roomOptions.MaxPlayers = System.Convert.ToByte(maxPlayers);
    //    roomOptions.EmptyRoomTtl = 60000;
    //    roomOptions.CustomRoomPropertiesForLobby = new string[] { MAP_PROP_KEY, MODE_PROP_KEY };
    //    roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable {
    //        { MAP_PROP_KEY, map},
    //        { MODE_PROP_KEY, mode}
    //    };

    //    PhotonNetwork.CreateRoom(name, roomOptions, null);

    //    //UpdateLobbyListings();
    //}

    #endregion


}
