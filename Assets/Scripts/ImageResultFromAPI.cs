using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// This script will send the PNG to the Image API for predictions of what the object is
/// It does this by submitting the byte array to the API and parsing the response, then
/// showing the result with the highest confidence.
///
/// References:
/// (1) Imagga's Tagging API: https://docs.imagga.com/#tags
/// (2) Unity Web Request Post Documentation: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html
/// </summary>
public class ImageResultFromAPI : MonoBehaviour
{
    //This textmesh refers to the first line of text on the SphereWithText Prefab:
    [SerializeField] private TextMesh TextDisplay;
    //The byte array that can be modified so a request can be done:
    public byte[] currentByteArray;
    //Stores the name of the object after a response from the server.
    //Always stores the name with the highest confidence:
    public string CurrentObjectName = string.Empty;

    //The base API call url for Imagga:
    private readonly string baseImageURL = "https://api.imagga.com/v2/tags";
    //Holds a Private reference to the Translation API so that it can call it to translate
    //once the object has been identified:
    private TranslationAPI m_TranslationAPI;

    private void Start()
    {
        //Cache the component for future use:
        m_TranslationAPI = GetComponent<TranslationAPI>();
        //Because Imagga only accepts Base64 encoded strings in its requests,
        //a Conversion is first done to the Byte Array:
        var base64String = Convert.ToBase64String(currentByteArray);
        StartCoroutine(GetImageData(base64String));
        //This changes the text to inform the user that the application is
        //attempting to Identify the object:
        TextDisplay.text = "Identifying Object...";
    }

    private void Update()
    {
        //This makes all the obects face the player, which makes it more legible in AR mode:
        transform.LookAt(Camera.main.transform, Vector3.up);
    }

    private IEnumerator GetImageData(string data)
    {
        //This uses a List of FormSelection data, which allows for Form-Data bodies to be
        //submitted along with the post requests. See (1) for API documentation:
        //This form data attaches the base64 string to it:
        var formData = new List<IMultipartFormSection> {new MultipartFormDataSection("image_base64", data)};
        //Creates a new disposable instance of UnityWebRequest, with a POST http Action:
        using var webRequest = UnityWebRequest.Post(baseImageURL, formData) ;
        
        //Set the Authorization Header for the API to know that a valid developer is using this API:
        webRequest.SetRequestHeader("Authorization",
            APIKeyConstants.IMAGEAPI_KEY);
        //Send the webrequest and freeze the coroutine until a result returns
        yield return webRequest.SendWebRequest();

        //If the returning webRequest is not 200 (Success), stop the coroutine and report it to the user
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            TextDisplay.text = "Connection Error. Please try again.";
            //Destroy the gameobject after 5 seconds as it serves no purpose:
            Destroy(gameObject, 5f);
            yield break;
        }
        //If it passes, parse the request result
        var imageInformation = JSON.Parse(webRequest.downloadHandler.text);
        //The results will all be contained in the Json under the Result node, with its "tags"
        //which represent all the possible predictions:
        var results = imageInformation["result"]["tags"];

        //always take the first child, as it is always the one with highest confidence:
        var firstResult = results.Children.First();
        //Get English name of the result:
        var identification = firstResult["tag"]["en"].Value;
        //Get the confidence value:
        var confidence = float.Parse(firstResult["confidence"]);

        //If the confidence produced by the algorithm is below 40%, inform the user
        //that the prediction will most likely be inaccurate
        if (confidence < 40)
        {
            TextDisplay.text = "Unsure of item's identity, move closer to the item and tap on it.";
            //Destroy the gameobject after 5 seconds as it serves no purpose:
            Destroy(gameObject, 5f);
            yield break;
        }
        //Inform the user about the prediction:
        TextDisplay.text = $"English: {identification} (Confidence: {confidence:0.0}%)";
        CurrentObjectName = identification;

        //When the object has been recognised, call the Translation API to translate the text:
        m_TranslationAPI.StartTranslation(CurrentObjectName, TranslationManager.Instance.ReturnSelectedLanguageInfo());
    }

    //This pulic method allows for a translation to happen again if the user switches languages:
    public void RefreshTranslations()
    {
        //This is for error protection, if an object fails recognition, any Language Refresh should not include this
        //object
        if (CurrentObjectName.Equals(string.Empty)) return;
        m_TranslationAPI.StartTranslation(CurrentObjectName, TranslationManager.Instance.ReturnSelectedLanguageInfo());
    }
}