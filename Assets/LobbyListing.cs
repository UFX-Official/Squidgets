
public class LobbyListing 
{
    public string lobbyName;
    public GameMode lobbyGameMode;
    public Map lobbyMap;
    public uint currentPlayerCount, maxPlayerCount;

    public LobbyListing(string lobbyName, GameMode gameMode, Map map, uint maxPlayers)
    {
        this.lobbyName = lobbyName;
        lobbyGameMode = gameMode;
        lobbyMap = map;
        currentPlayerCount = 0;
        maxPlayerCount = maxPlayers;        
    }
    
}
