using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSettings
{
    public string lobbyName;
    public GameMode.Mode lobbyGameMode;
    public GameMap.Map lobbyMap;
    public uint currentPlayerCount, maxPlayerCount;

    public RoomSettings(string lobbyName, GameMode.Mode gameMode, GameMap.Map map, uint maxPlayers)
    {
        this.lobbyName = lobbyName;
        lobbyGameMode = gameMode;
        lobbyMap = map;
        currentPlayerCount = 0;
        maxPlayerCount = maxPlayers;
    }
}
