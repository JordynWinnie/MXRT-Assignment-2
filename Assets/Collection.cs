using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This Script represents one template collection element, part of the PhraseWithTranslation prefab
/// Which contains two texts, one for the English word and another for the translation
/// </summary>
public class Collection : MonoBehaviour, ITranslationDisplayResult
{
    [SerializeField] Text EnglishTranslationDisplay;
    [SerializeField] Text TargetLanguageTranslationDisplay;
    private TranslationAPI m_TranslationAPI;
    public string CollectionItemName;
    private void Start()
    {
        
        //Set the string to empty as the Translation has not begun:
        TargetLanguageTranslationDisplay.text = string.Empty;
        m_TranslationAPI = GetComponent<TranslationAPI>();
        //Get the Language from user preferences:
        var selectedLanguage = DataLoader.LanguageValues[DataLoader.GetUserDefaultLang()];
        EnglishTranslationDisplay.text = CollectionItemName;
        //Start the translation for the selected languauge:
        m_TranslationAPI.StartTranslation(CollectionItemName, selectedLanguage);
    }

    public void DisplayTranslationResult(TranslationModel translatedTextInfo)
    {
        //Set the result of the translation to the text:
        TargetLanguageTranslationDisplay.text = translatedTextInfo.TranslationResult;
        //If there is a transliteration, append the Transliteration in brackets:
        if (!translatedTextInfo.TransliterationResult.Equals(string.Empty))
        {
            TargetLanguageTranslationDisplay.text += $" ({translatedTextInfo.TransliterationResult})";
        }
    }

    public void DisplayMessage(string message)
    {
        TargetLanguageTranslationDisplay.text = message;
    }

    public void DisplayError(string message)
    {
        TargetLanguageTranslationDisplay.text = message;
    }
}
