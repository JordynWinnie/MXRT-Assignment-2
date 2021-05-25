
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public static class JsonDataLoader
{
    private static List<KeyValuePair<string, string>> LanguageValues;

    public static string GetLanguagePath()
    {
        return Application.persistentDataPath + "/LanguageCaches.json";
    }
    
    public static List<KeyValuePair<string, string>> GetLanguages()
    {
        LanguageValues = new List<KeyValuePair<string, string>>();
        foreach (var language in GetLanguageData())
        {
            LanguageValues.Add(new KeyValuePair<string, string>(language.Key, language.Value));
        }
        return LanguageValues;
    }
    
    public static bool CheckIfFileExists()
    {
        return File.Exists(GetLanguagePath());
    }

    public static void WriteToJson(string data)
    {
        File.WriteAllText(GetLanguagePath(), data);
    }

    public static JSONNode GetLanguageData()
    {
        return JSON.Parse(File.ReadAllText(GetLanguagePath()));
    }
}
