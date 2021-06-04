/// <summary>
/// This class is similar in concept to ITranslationDisplay, but for the results of Item Recognition:

/// </summary>
public interface IDisplayResult
{
    public void DisplayMessage(string message);
    public void DisplayError(string message);
}
