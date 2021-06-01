using System.Collections;
using System.Text;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// This script is responsible for Downloading all available languages that can be translated
/// from the Microsoft Translate API, noted here:
/// https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-languages
///
/// The Script downloads all available languages, check if they require Transliteration,
/// then packages all that information into a file that is stored by the Device.
/// This is so the user does not always have to download available languages if he has already done it
///
/// References:
/// (1) This project uses the JsonLibrary called SimpleJSON:
/// https://wiki.unity3d.com/index.php/SimpleJSON which is a full C#-only Json parser,
/// meaning it works well with all Unity builds crossplatform with no issue.
/// (2) An Article about making users wait on purpose: https://uxmag.com/articles/let-your-users-wait
/// more information can be found about this phenomenon by Googling "using artificial delays in UX"
/// </summary>
public class DataDownloader : MonoBehaviour
{
    //Represents the Text that displays information to the user about the condition of his download:
    [SerializeField] private TextMeshProUGUI FilePathCheck;
    [SerializeField] private Button ForceRetry;

    //This is API Link, as mentioned above that gives information about the languages that can be translated
    //along with whether they require transliteration:
    private readonly string baseURL =
        "https://microsoft-translator-text.p.rapidapi.com/languages?api-version=3.0&scope=translation%2Ctransliteration";


    private void Start()
    {
        FilePathCheck.text = "Checking Data Files...";
        //On App Startup, run CheckForJsonData() to check if the user has downlaoded the data:
        CheckForJsonData();
        //DataLoader.AppendCollectionToJson("Hello");
        Debug.Log(DataLoader.UserCollectionValues);
    }

    private void CheckForJsonData()
    {
        //This checks if the LanguageCache.json file is currently in the user's phone:
        //More Information in JsonDataLoader.cs:
        if (DataLoader.CheckIfFileExists(DataLoader.GetLanguagePath()))
        {
            //If the file exists, allow the user to press the StartAR button
            //and inform the user about how many languages are loaded from his phone
            ForceRetry.interactable = true;
            FilePathCheck.text = $"Managed to load {DataLoader.LanguageValues.Count} languages.";
            //Exit the function to prevent redownloading of the file:
            //Load the MainMenu after a 1 second delay:
            StartCoroutine(StartMainMenu());
            return;
        }
        //If File is not found, language list download should be attempted,
        //and the user will be prevented from Starting AR:
        ForceRetry.interactable = false;
        StartCoroutine(GetLanguages());
    }

    private IEnumerator GetLanguages()
    {
        //Create a new Disposable instance of UnityWebRequest.
        //Set the HTTP request to be GET with the BaseUrl:
        using var languageGetRequest = UnityWebRequest.Get(baseURL);
        //Update the user to let them know a download is happening:
        FilePathCheck.text = "Downloading File...";
        //Set the required headers as per the API's needs. 
        //This uses RapidAPI's Wrapper of the Microsoft Translation API:
        languageGetRequest.SetRequestHeader("x-rapidapi-key", APIKeyConstants.RAPIDAPI_KEY);
        languageGetRequest.SetRequestHeader("x-rapidapi-host", "microsoft-translator-text.p.rapidapi.com");
        //This lets the request know to expect to recieve/send Json Data:
        languageGetRequest.SetRequestHeader("content-type", "application/json");
        //This lets the server know not to encode the data specifically to any format.
        //Unity will warn that this behavior might cause some issues, but this helps 
        //Android Builds parse the UTF-8 characters without error:
        languageGetRequest.SetRequestHeader("Accept-Encoding", string.Empty);

        //Send the WebRequest to attempt download, the coroutine will halt
        //running any code below until the request succeeds:
        yield return languageGetRequest.SendWebRequest();

        //When the web request succeeds, check the Result of the Request,
        //when there is bad internet connection, or the servers cannot be reached,
        //The user will be informed:
        if (languageGetRequest.result != UnityWebRequest.Result.Success)
        {
            FilePathCheck.text = "Error Downloading File. Check your internet connection, and try again.";
            ForceRetry.interactable = true;
        }

        //Gets the Raw byte data result from download
        var languageData = languageGetRequest.downloadHandler.data;
        //Encodes the Byte data into a string that can be parsed in JSON:
        var languageListResults = Encoding.UTF8.GetString(languageData);
        //Parses the JSON, Refer to (1)
        var languages = JSON.Parse(languageListResults);

        //Gets the Translations and Transliterations by going through the nodes:
        //Refer to (1) for Json Information.
        var transliterations = languages["transliteration"];
        var translations = languages["translation"];
        //Create the Json that will eventually be stored into a file:
        var languageNames = new JSONObject();
        //Iterate through all the ChildNodes of Translation: 
        //Refer to TranslationPairSampleResponse.json to see the a sample of a response:
        foreach (var child in translations)
        {
            //Get the Language Code, which is the ISO 639-1 code representation of the language.
            //More information in LanguageModel.cs:
            var langCode = child.Key;

            //Skip English Translation as translations will always be from English > Another Language
            if (langCode == "en")
                continue;
            //Get the languages' display name, which is a Human friendly representation of the Language, i.e Japanese, Korean etc.
            var langName = child.Value["name"].Value;
            //Add the information to the JsonObject:
            languageNames.Add(langCode, langName);
        }

        foreach (var child in transliterations)
        {
            //Iterate through all trasliterations and get their language code:
            var langCode = child.Key;

            //Since transliteration may sometimes have languages/script that currently do not exist in the Translation
            //API, for instance Kyrgyz, this will prevent runtime errors:
            if (languageNames[langCode] != null)
            {
                //If it finds a valid script for a language, it will parse to get that script value:
                var langScript = child.Value["scripts"][0]["code"].Value;
                //This then changes the current language to support a script as well,
                //by appending a semi-colon and the script.
                //This will be further parsed by JsonDataLoader.cs:
                languageNames[langCode] += $";{langScript}";
            }
        }
        //Calls the data loader to write the LanguageCache to Json in the device files:
        DataLoader.WriteLanguagesToJson(languageNames.ToString());
        //Calls for another check, which should enable the AR button if successful:
        CheckForJsonData();
    }
    //A button call to force deletion of data, and re-download, 
    //this is for any corrupted files, or if the user knows that more languages are
    //supported by Microsoft's Translation API:
    public void DeleteDataAndCheckAgain()
    {
        DataLoader.DeleteLanguageFile();
        CheckForJsonData();
    }

    //Add an artifical delay to make the user feel more assured
    //Reference to (2)
    IEnumerator StartMainMenu()
    {
        yield return new WaitForSeconds(0.75f);

        if (DataLoader.CheckIfUserHasDefaultLang())
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(3);
        }
        
    }

}