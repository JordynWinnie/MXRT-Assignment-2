using System;
using UnityEngine;
/// <summary>
/// This Script acts as a wrapper for the ITranslationDisplay interface, allowing the TranslationAPI to send
/// results to this script:
/// </summary>
public class ARObject : MonoBehaviour, ITranslationDisplayResult
{
    
    [SerializeField] private TextMesh TranslatedText;

    private void Start()
    {
        //The Translated text should start as empty as the object has not been indentified:
        TranslatedText.text = string.Empty;
    }

    public void DisplayTranslationResult(TranslationModel translatedTextInfo)
    {
        //Set the text to be the Language name followed by the translation:
        TranslatedText.text =
            $"{translatedTextInfo.LanguageInformation.LanguageDisplayName}: {translatedTextInfo.TranslationResult}";
        
        //If there is a transliteration, append the Transliteration in brackets:
        if (!translatedTextInfo.TransliterationResult.Equals(string.Empty))
        {
            TranslatedText.text += $" ({translatedTextInfo.TransliterationResult})";
        }
    }

    //The following two methods are just the base implementation of the ITranslationDisplayResult interface
    //It will set the current message to whatever message is given by the TranslationAPI,
    //which can inform the user of actions or errors if required:
    public void DisplayMessage(string message)
    {
        TranslatedText.text = message;
    }

    public void DisplayError(string message)
    {
        TranslatedText.text = message;
    }
}
