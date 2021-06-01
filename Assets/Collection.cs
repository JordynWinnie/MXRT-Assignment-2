using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This Script represents one template collection element, part of the PhraseWithTranslation prefab
/// Which contains two texts, one for the English word and another for the translation
/// </summary>
public class Collection : MonoBehaviour
{
    [SerializeField] Text EnglishTranslationDisplay;
    private TranslationAPI m_TranslationAPI;
    public string CollectionItemName;
    private void Start()
    {
        m_TranslationAPI = GetComponent<TranslationAPI>();
        //Get the Language from user preferences:
        var selectedLanguage = DataLoader.LanguageValues[DataLoader.GetUserDefaultLang()];
        EnglishTranslationDisplay.text = CollectionItemName;
        //Start the translation for the selected languauge:
        m_TranslationAPI.StartTranslation(CollectionItemName, selectedLanguage);
    }
}
