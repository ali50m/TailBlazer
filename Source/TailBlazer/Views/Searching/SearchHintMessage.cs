using System.ComponentModel;

namespace TailBlazer.Views.Searching;

public class SearchHintMessage :INotifyPropertyChanged
{
    public static readonly SearchHintMessage Valid = new SearchHintMessage(true, null);

    public bool IsValid { get; }
    public string Message { get; }

    public SearchHintMessage(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }

    //implemented to prevent memory leaks
    public event PropertyChangedEventHandler PropertyChanged;
}