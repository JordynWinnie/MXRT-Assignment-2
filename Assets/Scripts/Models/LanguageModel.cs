/// <summary>
/// This class serves as a way to easily store information about each language upon reading it
/// from the JSON file.
///
/// References:
/// (1) ISO 639-1 codes https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
/// (2) Microsoft Transliteration API: https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-transliterate 
/// </summary>
public class LanguageModel
{
    //This is the internal code used by the external API to determine the Language to translate to and from
    //It uses ISO 639-1 codes (refer to Ref 1), to uniquely identify all the world's Languages.
    //Example: ja, en
    public string LanguageCode;
    //This represents a more human friendly and readable version of the language name, this will
    //be displayed to users for them to choose a language
    //Example: Japanese, English 
    public string LanguageDisplayName;
    //This optional field represents that the language has a script. Generally, Asian languages
    //like Chinese, Japanese, Hindi have non-roman script systems. This is used
    //by the Microsoft Transliteration API to convert script-based languages to their 
    //roman counter parts. Like making 你好 into NiHao, more info at Reference 2
    //Example: Jpan, Latn (Representing Japanese Script, and Latin Script)
    public string LanguageScript;
    
    //This default constructor makes a basic Language model, with a default field for Languagescript
    //as not all languages are scripted. Like for instance, French, still uses Latin Characters
    public LanguageModel(string languageCode, string languageDisplayName, string languageScript = "")
    {
        LanguageCode = languageCode;
        LanguageDisplayName = languageDisplayName;
        LanguageScript = languageScript;
    }
}