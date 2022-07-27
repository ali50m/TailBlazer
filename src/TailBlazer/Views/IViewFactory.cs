using TailBlazer.Domain.Settings;
using TailBlazer.Infrastucture;

namespace TailBlazer.Views;

public interface IViewModelFactory
{
    string Key { get; }
    HeaderedView Create(ViewState state);
}