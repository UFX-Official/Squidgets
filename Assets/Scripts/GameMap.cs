public static class GameMap
{
    public enum Map
    {
        DevTower = 0
    }

    public static Map GetMapByID(int id)
    {
        return (Map)id;
    }

    public static Map GetMapByName(string name)
    {
        return (Map)System.Enum.Parse(typeof(Map), name);
    }

    public static int GetIDByMap(Map map)
    {
        return (int)map;
    }

    public static int GetIDByName(string name)
    {
        return (int)System.Enum.Parse(typeof(Map), name);
    }

    public static string GetNameByMap(Map map)
    {
        return map.ToString();
    }

    public static string GetNameByID(int id)
    {
        return ((Map)id).ToString();
    }

}