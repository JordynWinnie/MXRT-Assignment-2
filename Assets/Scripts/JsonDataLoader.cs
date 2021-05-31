using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;
/// <summary>
/// This is a static class that handles all the Language Loading and Caching
/// in the application
/// It is able to store downloaded languages to the Device, and is able to read from it as well
/// This acts as a central point for getting all the languages, therefore it is a static
/// class that can be accessed anywhere in the code:
///
/// References:
/// (1) Unity Application.persistentDataPath: https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
/// </summary>
public static class JsonDataLoader
{
    //Private Reference of a LanguageModel list, which can be changed within this file:
    private static List<LanguageModel> _languageValueReference = new List<LanguageModel>();
    //Create a ReadOnly List of LanguageModel, it will reference a private loaded
    //language reference, which can be changed throughout this code
    //Also, if the current language list is empty, refresh the list, otherwise return the private reference
    //This prevents constantly re-creating "new" list which can waste memory, especially on a Mobile Device:
    public static List<LanguageModel> LanguageValues
    {
        get
        {
            if (_languageValueReference.Count == 0)
            {
                _languageValueReference = LoadLanguageList();
            }

            return _languageValueReference;
        }
    }


    //Helper method that returns a path to the LanguageCache files in the User's Device.
    //Application.persistentDataPath is a Unity helper method that will return a different path
    //to save Persistent data on every platform.
    //iOS: var/mobile/Containers/Data/Application/<guid>/Documents.
    //Andriod: storage/emulated/0/Android/data/<packagename>/files 
    //For more information refer to Ref 1:
    private static string GetLanguagePath() => Application.persistentDataPath + "/LanguageListCache.json";
    
    //This method allows for deletion of LanguageFiles incase any corruption happens
    public static void DeleteLanguageFile()
    {
        //It checks if the file exists:
        if (CheckIfFileExists())
        {
            //If it does, it will attempt to delete the file:
            File.Delete(GetLanguagePath());
            
            //Clear the items, so the next call to LanguageValues will be forced
            //to call LoadLanguageList()
            _languageValueReference.Clear();
        }
    }
    
    //This is a helper method that creates a new LanguageModel list, 
    //reads the Json data from the Device,
    //Then adds it into the list:
    private static List<LanguageModel> LoadLanguageList()
    {
        var languageList = new List<LanguageModel>();
        //Read through all the languages from the JSON file:
        foreach (var language in GetLanguageData())
        {
            //For the Json, it is formatted as LanguageCode (Key) : LanguageDisplayName (Value) ;(Optional) Language Script:
            //this is explained in more detail in DataDownloader.cs.
            
            /* Gets the Display Language as First Element, and an optional
             script for transliteration as the second Element.
             It does this by Splitting it by the Delimiter of a Semi-Colon:
            */
            var languageRaw = language.Value.Value.Split(';');
            //Creates a new LanguageModel, which uses the LangCode as key, and the DisplayName as the first element in the split 
            //list
            var model = new LanguageModel(language.Key, languageRaw[0]);
            //When there is more than 1 thing in the String Array, it means that 
            //there is an optional script;
            if (languageRaw.Length > 1)
                //Set the current model language's LanguageScript to the second item in the Split Array:
                model.LanguageScript = languageRaw[1];
            //Add that Model to that list:
            languageList.Add(model);
        }
        //Return the newly created list:
        return languageList;
    }

    //This uses the C# helper method to see if a File Exists:
    public static bool CheckIfFileExists() => File.Exists(GetLanguagePath());
    //Helper method to Write the JsonLanguage file to the path with the current data:
    public static void WriteToJson(string data) => File.WriteAllText(GetLanguagePath(), data);
    
    //Internal method that Parses the Json from the Language file stored on the device:
    private static JSONNode GetLanguageData() => JSON.Parse(File.ReadAllText(GetLanguagePath()));
    
}