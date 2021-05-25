using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public static class JsonDataLoader
{
    private static List<LanguageModel> LanguageValues;

    public static string GetLanguagePath()
    {
        return Application.persistentDataPath + "/LanguageListCache.json";
    }

    public static void DeleteLanguageFile()
    {
        File.Delete(GetLanguagePath());
    }

    public static List<LanguageModel> LoadLanguageList()
    {
        LanguageValues = new List<LanguageModel>();
        foreach (var language in GetLanguageData())
        {
            /* Gets the Display Language as First Element, and an optional
             script for transliteration as the second Element.
            */
            var languageRaw = language.Value.Value.Split(';');
            var model = new LanguageModel(language.Key, languageRaw[0]);
            if (languageRaw.Length > 1)
                //This means that the current language has an available Transliteration script:
                model.LanguageScript = languageRaw[1];
            LanguageValues.Add(model);
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