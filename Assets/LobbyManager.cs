using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
  
    #region Debug
    
        string gameVersion = "v0.0.5";
        string versionTimestamp = "Oct. 22, 2020";
        string copyright = "Halfwit Heroes©";

        [SerializeField] private Text loadingDisplay = null;
        [SerializeField] private Text versionDisplay = null;
    
    #endregion
    
    #region MonoBehaviour Callbacks

        private void Awake()
        {
            Screen.fullScreen = false;        
        }

        private void Start()
        {
            // Attempt to Connect To Server
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            loadingDisplay.text = "Connecting to Server...";
        
            // Load Player Prefs if available.            
            if (PlayerPrefs.HasKey("NickName")) // Check to see if the a nickname is stored in player prefs
            {
                Debug.Log("Nickname Found!");
                loginNameField.text = PlayerPrefs.GetString("NickName");
            }            
        }    

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks
    
        public override void OnConnectedToMaster()
        {
            Debug.Log("LobbyManager: We are now connected to the " + PhotonNetwork.CloudRegion + " server!");
            PhotonNetwork.AutomaticallySyncScene = true;

            #region Connection Display Text

            loadingDisplay.text = "Connected to " + PhotonNetwork.CloudRegion + " server.";
            versionDisplay.gameObject.SetActive(true);
            versionDisplay.text = "version " + gameVersion + ", " + versionTimestamp + ". " + copyright;

            #endregion
        
            #region Open Login Panel

            loginPanel.SetActive(true);

            #endregion

            roomListings = new List<RoomInfo>(); // See [r]Lobby Management        

        }

        public override void OnJoinedLobby()
        {
            loadingDisplay.text = "Connected to " + PhotonNetwork.CurrentLobby.Name + " as " + PhotonNetwork.NickName + "  via " + PhotonNetwork.CloudRegion + " server.";
        }

        public override void OnLeftLobby()
        {
            loadingDisplay.text = "Connected to " + PhotonNetwork.CloudRegion + " server.";
        }
    
    #endregion

    #region Login Panel

        #region Login Panel Components

            [Header("Login Panel Components")]
            [SerializeField] GameObject loginPanel = null;
            [SerializeField] InputField loginNameField = null;
            [SerializeField] Text loginInstructionDisplay = null;

        #endregion

        #region Login Panel Custom Methods

            public void Login()
            {
                // Confirm the name isn't blank.
                if (loginNameField.text == "")
                {
                    loginInstructionDisplay.color = Color.red;
                    loginInstructionDisplay.text = "Name cannot be left blank.  Please insert a name.";
                    return;
                }
        
                // If PlayerPrefs detects a nickname, set it by default
                if (PlayerPrefs.HasKey("NickName"))
                {
                    Debug.Log("NickName found! Welcome Back!");
                    PlayerPrefs.SetString("NickName", loginNameField.text);
                }

                PhotonNetwork.NickName = loginNameField.text;

                // Join the Lobby
                loginPanel.SetActive(false);
                lobbyPanel.SetActive(true);

                Debug.Log("Joining Lobby");

                TypedLobby lobby = new TypedLobby("Main Lobby", LobbyType.Default);
                PhotonNetwork.JoinLobby(lobby);        
            }

            public void Logout()
            {
                // Leave the Lobby
                loginPanel.SetActive(true);
                lobbyPanel.SetActive(false);

                Debug.Log("Leaving Lobby.");
                PhotonNetwork.LeaveLobby();
            }

            public void Quit()
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;            
                #else
                    Application.Quit();
                #endif        
            }

        #endregion
    
    #endregion

    #region Lobby Panel

    [Header("Lobby Panel")]
    [SerializeField]
    private GameObject lobbyPanel = null;
    public Transform roomListParent = null;
    public GameObject noLobbyText = null;

    
    private List<RoomInfo> roomListings;

    // Currently Disabled
    #region Lobby Chat Panel

        //[Header("Chat Variables")]
        //[SerializeField] Transform chatParent = null;
        //[SerializeField] InputField chatInputField = null;

        //public void SendChatInput()
        //{
        //    PhotonView photonView = PhotonView.Get(this);
        //    photonView.RPC("SendMessageToChat", RpcTarget.All, PhotonNetwork.NickName, chatInputField.text);
        //}

        //[PunRPC]
        //private void SendMessageToChat(string playerName, string message)
        //{
        
        //    GameObject _message = PhotonNetwork.Instantiate("Prefabs/UI/ChatTextPrefab", Vector3.zero, Quaternion.identity, 0);

        //    _message.GetComponent<Text>().text = playerName + ": " + message;
        //    _message.transform.parent = chatParent.parent;
        
        //}

    #endregion




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
            GameObject listing = Instantiate(Resources.Load("Prefabs/UI/RoomListing"), roomListParent) as GameObject;
            RoomListingButton button = listing.GetComponent<RoomListingButton>();

            //string modeInput = room.CustomProperties["Mode"].ToString();
            //string mapInput = room.CustomProperties["Map"].ToString();
            int modeInput = 0;
            int mapInput = 0;

            button.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount, modeInput, mapInput);
        }
    }

    #endregion

    #region Room Creation Panel

        #region Room Creation Components

        [Header("Room Creation Components")]
        [SerializeField] private GameObject roomCreationPanel = null;

        public InputField roomNameInput = null;
        public Dropdown roomGameModeInput = null;
        public Dropdown roomMapInput = null;
        public Dropdown roomMaxPlayerInput = null;

        #endregion

        #region Room Creation Variables

        public const string MAP_PROP_KEY = "MAP";
        public const string MODE_PROP_KEY = "MODE";

        #endregion

        #region Custom Room Creation Methods

        public void CreateRoom()
        {
            string name = roomNameInput.text;
            GameMode.Mode mode = GameMode.GetModeByID(roomGameModeInput.value);
            GameMap.Map map = GameMap.GetMapByID(roomMapInput.value);
            uint maxPlayers = (uint)roomMaxPlayerInput.value + 1;

            RoomOptions options = new RoomOptions()
            {
                IsVisible = true,
                IsOpen = true,
                EmptyRoomTtl = 3000,
                MaxPlayers = System.Convert.ToByte(maxPlayers),
                CustomRoomPropertiesForLobby = new string[] { MODE_PROP_KEY, MAP_PROP_KEY },
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { MODE_PROP_KEY, mode }, { MAP_PROP_KEY, map } }
            };

            PhotonNetwork.CreateRoom(name, options);
        }
    
        #endregion 

        #region PUN Room Creation Callbacks

        public override void OnCreatedRoom()
        { 
            // Switch to Room Info Panel
            roomCreationPanel.SetActive(false);
            roomInfoPanel.SetActive(true);        
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("Room Creation failed with error code {0} and error message {1}", returnCode, message);
        }

        #endregion
    

    #endregion

    #region Room Info Panel

        #region Room Info Components

            [Header("Room Info Panel")]
            [SerializeField]
            private GameObject roomInfoPanel = null;
    
            // Room Info
            public Text roomNameDisplay = null;
            public Text roomModeDisplay = null;
            public Text roomMapDisplay = null;
            public Text roomCountDisplay = null;
    
            // Room Settings
            public InputField roomNameChangeInput = null;
            public Dropdown roomModeChangeInput = null;
            public Dropdown roomMapChangeInput = null;
            public Dropdown roomCountChangeInput = null;
        
            // Start Button
            [SerializeField] private Button startGameBtn = null;
        
            // Room Player List
            [SerializeField] private Transform roomPlayerListParent = null;

    #endregion

    // Currently Disabled
    #region Room Chat Panel

        [Header("Room Chat Variables")]
        [SerializeField] Transform chatParent = null;
        [SerializeField] InputField chatInputField = null;

        public void SendChatRoomInput()
        {
            if (chatInputField.text == "")
            {
                chatInputField.DeactivateInputField();
            }   
            else
            {
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("SendMessageToChatRoom", RpcTarget.All, PhotonNetwork.NickName, chatInputField.text);

                chatInputField.text = "";
                chatInputField.Select();
                chatInputField.ActivateInputField();            
            }
        }

        [PunRPC]
        private void SendMessageToChatRoom(string playerName, string message)
        {

            GameObject _message = PhotonNetwork.Instantiate("Prefabs/UI/ChatTextPrefab", Vector3.zero, Quaternion.identity, 0);
            _message.GetComponent<Text>().text = playerName + ": " + message;
            _message.transform.SetParent(chatParent.transform);
            _message.transform.localScale = Vector3.one;
        }

    #endregion

    #region Room Info Variables

    [SerializeField]
            private int multiPlayerSceneIndex = 0;

        #endregion

        #region Custom Room Info Methods

        // Button Input for Room Master
        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;  // Hide's the game from the room listings while playing.  Comment this out if you want players to join mid game.
                PhotonNetwork.LoadLevel(1 + roomMapChangeInput.value); // 1 is the starting scene index for maps.  This will change as more scenes are added to the game.
            }
        }

        // Button Input for Leaving the current room
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    
        // Clears the list of players within the room.
        public void ClearPlayerListings()
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
                GameObject listing = Instantiate(Resources.Load("Prefabs/UI/PlayerListing"), roomPlayerListParent) as GameObject;
                Text nameDisplay = listing.transform.GetChild(0).GetComponent<Text>();
                nameDisplay.text = player.NickName;
            }
        }

        #region Room Info Edit Settings
        // For use by the Room Manager to adjust game settings
        
            public void ToggleRoomModeChange(bool state)
            {
                if (PhotonNetwork.IsMasterClient)
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
            }
            public void SetRoomMode()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    roomModeDisplay.text = "Gamemode: " + GameMode.GetNameByID(roomModeChangeInput.value);
                    PhotonNetwork.CurrentRoom.PropertiesListedInLobby[0] = GameMode.GetNameByID(roomModeChangeInput.value);
                    ToggleRoomModeChange(false);
                }
            }

            public void ToggleRoomMapChange(bool state)
            {
                if (PhotonNetwork.IsMasterClient)
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
        
            }
            public void SetRoomMap()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    roomMapDisplay.text = "Map: " + GameMap.GetNameByID(roomMapChangeInput.value);                    
                    multiPlayerSceneIndex = 1 + roomMapChangeInput.value;
            
                    PhotonNetwork.CurrentRoom.PropertiesListedInLobby[1] = GameMap.GetNameByID(roomMapChangeInput.value);
                    ToggleRoomMapChange(false);
                }
            }

            public void ToggleRoomCountChange(bool state)
            {
                if (PhotonNetwork.IsMasterClient)
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
            }
            public void SetRoomCount()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    roomCountDisplay.text = "Player Count: " + PhotonNetwork.CurrentRoom.Players.Count + " / " + (roomCountChangeInput.value + 1).ToString();
                    PhotonNetwork.CurrentRoom.MaxPlayers = System.Convert.ToByte(roomCountChangeInput.value + 1);
                    ToggleRoomCountChange(false);
                }
            }

            #endregion

        #endregion
    
        #region PUN Room Info Callbacks

        public override void OnJoinedRoom()
        {

            #region Switch UI from Lobby to Room 

            lobbyPanel.SetActive(false);
            roomInfoPanel.SetActive(true);

            // Set Room Info Panel Information 
            roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
            
            object _map, _mode;
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MAP", out _map);
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MODE", out _mode);

            roomModeDisplay.text = "Gamemode: " + GameMode.GetNameByID((int)_mode);
            roomMapDisplay.text = "Map: " + GameMap.GetNameByID((int)_map);
            roomCountDisplay.text = "Player Count: " + PhotonNetwork.CurrentRoom.Players.Count + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        loadingDisplay.text = "Connected to " + PhotonNetwork.CurrentRoom.Name + " as " + PhotonNetwork.NickName + " via " + PhotonNetwork.CloudRegion + " server.";
        
            #endregion

            #region Room Controls

            if (PhotonNetwork.IsMasterClient)
            {
                // Enable things that only the owner of the room can see
                /// i.e. Room Settings

                startGameBtn.interactable = true;
                startGameBtn.GetComponentInChildren<Text>().text = "Start Game";
            }
            else
            {
                // Enable things that only room members can see or hide things that they shouldnt see
                startGameBtn.interactable = false;
                startGameBtn.GetComponentInChildren<Text>().text = "Waiting For Host..";
            }

            #endregion

            // Refresh the Player List
            ClearPlayerListings();
            ListPlayers();
        }

        public override void OnLeftRoom()
                {
            #region Switch UI from Room to Lobby

            lobbyPanel.SetActive(true);
            roomInfoPanel.SetActive(false);

            loadingDisplay.text = "Connected to " + PhotonNetwork.CurrentLobby.Name + " as " + PhotonNetwork.NickName + " via " + PhotonNetwork.CloudRegion + " server.";

            #endregion
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
                startGameBtn.interactable = true;
                startGameBtn.GetComponent<Text>().text = "Start Game";
            }
        }

        #endregion
    
    #endregion
    
}
