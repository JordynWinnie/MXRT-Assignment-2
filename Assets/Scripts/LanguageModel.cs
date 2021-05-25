public class LanguageModel
{
    public string LanguageCode;
    public string LanguageDisplayName;
    public string LanguageScript;

    public LanguageModel(string languageCode, string languageDisplayName, string languageScript = "")
    {
        LanguageCode = languageCode;
        LanguageDisplayName = languageDisplayName;
        LanguageScript = languageScript;
    }
}