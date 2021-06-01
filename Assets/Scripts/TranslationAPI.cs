using System.Collections;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// This script handles the calls to the Translation API
/// The API used is the Microsoft Translate API, but wrapped with RapidAPI's abstraction layer. (Refer to 1)
/// This script will attempt to do two things, Translate and Transliterate (if necessary)
///
/// (1) RapidAPI with Microsoft Traslate API: https://rapidapi.com/microsoft-azure-org-microsoft-cognitive-services/api/microsoft-translator-text
/// (2) Microsoft Translator API Documentation: https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-translate
/// (3) Unity Discussion on the "Broken" Post Request: https://forum.unity.com/threads/unitywebrequest-post-url-jsondata-sending-broken-json.414708/
/// </summary>

public class TranslationAPI : MonoBehaviour
{
    //Accepts a TextField to change, or a TextMesh to modify with translation Data:
    [SerializeField] TextMesh TextMeshToChange;
    [SerializeField] Text TextToChange;

    private readonly string baseTranslateURL =
        "https://microsoft-translator-text.p.rapidapi.com/translate?api-version=3.0&textType=plain&profanityAction=NoAction";

    
    private readonly string baseTransliterateURL =
        "https://microsoft-translator-text.p.rapidapi.com/transliterate?toScript=Latn&api-version=3.0";

    private void Start()
    {
        //When the Prefab gets instantiated, it curently does not have any object information
        //so it should not display anything:
        UpdateTextFields(string.Empty);
    }


    //Helper method to change the different fields to the Translation message
    private void UpdateTextFields(string message)
    {
        //Check if the components are null, if not, change the text:
        if (TextMeshToChange)
        {
            TextMeshToChange.text = message;
        }

        if (TextToChange)
        {
            TextToChange.text = message;
        }
    }

    //This is called by ImageResultFromAPI.cs, when refreshing translations, or when the object has been recognised:
    public void StartTranslation(string textToTranslate, LanguageModel selectedLanguage)
    {
        //If a current Translation is running, and it gets Refreshed, it should cancel that translation, and use the new translation data:
        StopAllCoroutines();
       
        StartCoroutine(TranslateAndTransliterate(textToTranslate, selectedLanguage));
    }
    
    private IEnumerator TranslateAndTransliterate(string textToTranslate, LanguageModel selectedLanguage)
    {
        //These fields do not have to be variables, but are placed here to reduce code bloat:
        var targetLang = selectedLanguage.LanguageCode;
        var fromScript = selectedLanguage.LanguageScript;
        

        //This lets the user know a translation is being fetched:
        UpdateTextFields("Translating...");

        //Json data serialisation, as per the Microsoft Translate API guidelines, refer to (2)
        //It requires a Json Body with the Key of "text" and the data, which is what text needs to be translated
        //It has to be formatted as an Array of Json Objects, as the request can also accept multiple JSON objects:
        var translateData = new JSONArray();
        var translation = new JSONObject();
        translation.Add("Text", textToTranslate);
        translateData.Add(translation);

        //Url Interpretation, which converts it to a valid URL with the extra parameter of to: which allows
        //the application to query for the language that the user has selected:
        var translateFullURL = baseTranslateURL + "&to=" + targetLang;

        //Convert the prepared JSON data to bytes[] for POSTing
        var translateBodyRaw = Encoding.UTF8.GetBytes(translateData.ToString());
        //Since Post Method does not accept byte[] by default, a new WebRequest must be made
        //from scratch, this solution references a Forum Discussion, refer to (3)

        //This creates a UnityWebRequest that has it's own Upload and Downloadhandler:
        using var translatePostRequest = new UnityWebRequest(translateFullURL, "POST")
        {
            uploadHandler = new UploadHandlerRaw(translateBodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        //This informs the web server that we are sending over JSON data:
        translatePostRequest.SetRequestHeader("Content-Type", "application/json");
        //These two headers inform Rapid API which developer we are, and which API we would like to use:
        translatePostRequest.SetRequestHeader("x-rapidapi-key", APIKeyConstants.RAPIDAPI_KEY);
        translatePostRequest.SetRequestHeader("x-rapidapi-host", "microsoft-translator-text.p.rapidapi.com");

        //Send the Translation Request and suspend the coroutine unitl a result is returned:
        yield return translatePostRequest.SendWebRequest();

        //Check the Status code of the request, if it failed, inform the user:
        if (translatePostRequest.result != UnityWebRequest.Result.Success)
        {
            UpdateTextFields("Failed to connect to Translation Servers.");
            //unlike the Image Recognition version, do not destroy the current object
            //as there is still useful information present about the object
            yield break;
        }

        //Parse the Translation result and search through the tree for the translation
        //Do an additional trim on the " character as some languages have weird serialisations
        //when on the Android Platform:

        //An example of the Response will be in the TranslationResult.json. The sample uses this URL
        //https://microsoft-translator-text.p.rapidapi.com/translate?api-version=3.0&textType=plain&profanityAction=NoAction&to=ja
        //With the Post body of Text : Hello
        //Which translates Hello to Japanese:
        var translationJsonResult = JSON.Parse(translatePostRequest.downloadHandler.text);
        var translationResult = translationJsonResult[0]["translations"][0]["text"].Value.Trim('"');

        //Check if the current language has a script:
        if (fromScript.Equals(string.Empty))
        {
            //Display only translation, since Language does not require transliteration:
            UpdateTextFields($"{selectedLanguage.LanguageDisplayName}: {translationResult}");
            yield break;
        }

        //If the Translation Requires Transliteration, meaning Romanising the script,
        //an additional call to the API is done:
        //URL interpretation, it converts the base URL to a useable URL by supplying the script
        //that the source language is from, infroms the API what language it is
        //and in the base url that is also a Parameter toScript=Latn, which
        //requests that the script can converted to Latin charatcers for easy reading:
        var transliterateFullURL = $"{baseTransliterateURL}&fromScript={fromScript}&language={targetLang}";

        //The Process is similar to Translation:
        var transliterateData = new JSONArray();
        var transliteration = new JSONObject();
        transliteration.Add("Text", translationResult);
        transliterateData.Add(transliteration);
        var transliterateBodyRaw = Encoding.UTF8.GetBytes(transliterateData.ToString());
        using var transliteratePostRequest = new UnityWebRequest(transliterateFullURL, "POST")
        {
            uploadHandler = new UploadHandlerRaw(transliterateBodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        transliteratePostRequest.SetRequestHeader("Content-Type", "application/json");
        transliteratePostRequest.SetRequestHeader("x-rapidapi-key", APIKeyConstants.RAPIDAPI_KEY);
        transliteratePostRequest.SetRequestHeader("x-rapidapi-host", "microsoft-translator-text.p.rapidapi.com");
        yield return transliteratePostRequest.SendWebRequest();

        if (transliteratePostRequest.result != UnityWebRequest.Result.Success)
        {
            UpdateTextFields("Failed to connect to Translation Servers.");
            yield break;
        }
        
        var transliterationJsonResult = JSON.Parse(transliteratePostRequest.downloadHandler.text);

        var transliterationResult = transliterationJsonResult[0]["text"].Value.Trim('"');
        //Sets the result of the translation, along with the Latin Transliteration to help reading:
        UpdateTextFields($"{selectedLanguage.LanguageDisplayName}: {translationResult} ({transliterationResult})");
    }
}