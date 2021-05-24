
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public static class JsonDataLoader
{
    private static Dictionary<string, JSONNode> LanguageValues;

    public static string GetLanguagePath()
    {
        return Application.persistentDataPath + "/LanguageCaches.json";
    }
    
    public static void SetUpJsonLoader()
    {
        var orderByEnglishFirst = GetLanguageData().Linq.OrderByDescending(x => x.Key == "en");
        LanguageValues = new Dictionary<string, JSONNode>();
        foreach (var language in orderByEnglishFirst)
        {
            LanguageValues.Add(language.Key, language.Value.Value);
        }
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
