public static class GameMode
{
    public enum Mode
    {
        FreeRoam = 0,
        Deathmatch = 1,
        Elimination = 2,
        Obstacle = 3,
        Minigame = 4
    }

    public static Mode GetModeByID(int id)
    {
        return (Mode)id;
    }

    public static Mode GetModeByName(string name)
    {
        return (Mode)System.Enum.Parse(typeof(Mode), name);
    }

    public static int GetIDByMode(Mode mode)
    {
        return (int)mode;
    }

    public static int GetIDByName(string name)
    {
        return (int)System.Enum.Parse(typeof(Mode), name);
    }

    public static string GetNameByMode(Mode mode)
    {
        return mode.ToString();
    }

    public static string GetNameByID(int id)
    {
        return ((Mode)id).ToString();
    }
}
