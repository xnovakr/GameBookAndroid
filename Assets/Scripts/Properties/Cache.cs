using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cache
{
    public static string CACHEFILE = "ui.cache";

    public string language;
    public int fontSize;
    public Color backgroundColor;
    public Color fontColor;

    public Cache LoadCache()
    {
        char SLASH = '/';
#if UNITY_EDITOR
        SLASH = '\\';
#endif
        string cacheToLoad = File.ReadAllText(Application.persistentDataPath + SLASH + CACHEFILE);
        Cache loadedCache = JsonUtility.FromJson<Cache>(cacheToLoad);
        return loadedCache;
    }
    public void SaveCache(Cache savingCache)
    {
        char SLASH = '/';
#if UNITY_EDITOR
        SLASH = '\\';
#endif
        File.WriteAllText(Application.persistentDataPath + SLASH + CACHEFILE, JsonUtility.ToJson(savingCache));
    }
    public static bool CheckCacheFile()
    {//returns TRUE if file exist and FALSE if it desnt
        char SLASH = '/';
#if UNITY_EDITOR
        SLASH = '\\';
#endif
        return File.Exists(Application.persistentDataPath + SLASH + CACHEFILE);
    }
}
