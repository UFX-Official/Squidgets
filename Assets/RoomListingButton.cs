using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RoomListingButton : MonoBehaviour
{
    #region Private Variables

    private string roomName;
    private GameMode.Mode roomMode;
    private GameMap.Map roomMap;
    private int roomSize;
    private int playerCount;
    
    #endregion 

    #region UI Management

    [SerializeField] 
    private Text roomNameDisplay = null;

    [SerializeField]
    private Text roomModeDisplay = null;

    [SerializeField]
    private Text roomMapDisplay = null;

    [SerializeField]
    private Text roomSizeDisplay = null;

    #endregion

    #region Public Methods

    public void SetRoom(string nameInput, int sizeInput, int countInput, int modeInput, int mapInput)
    {
        roomName = nameInput;
        roomSize = sizeInput;
        playerCount = countInput;
        roomMode = GameMode.GetModeByID(modeInput);
        roomMap = GameMap.GetMapByID(mapInput);

        roomNameDisplay.text = nameInput;
        roomSizeDisplay.text = countInput + "/" + sizeInput;
        roomModeDisplay.text = GameMode.GetNameByID(modeInput);
        roomMapDisplay.text = GameMap.GetNameByID(mapInput);
    }

    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    #endregion



}
