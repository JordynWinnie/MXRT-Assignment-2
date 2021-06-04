    
public interface ITranslationDisplayResult : IDisplayResult
{
    /// <summary>
    /// This interface can be implemented by any class to recieve any information from the TranslationAPI,
    /// The translation API scipt automatically looks for an ITranslationDisplay to output the results to.
    ///
    /// This interface contains two main methods that have to be implemented.
    /// The message display, which will output progress of the translation API
    /// The error display, which will display the reason the translation failed
    /// and the Result, which will give a TranslationModel, which represents all the Translation and Transliteration information:
    /// </summary>

    public void DisplayTranslationResult(TranslationModel translatedTextInfo);
}