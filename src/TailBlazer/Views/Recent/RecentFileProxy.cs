using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using TailBlazer.Domain.FileHandling.Recent;

namespace TailBlazer.Views.Recent;

public class RecentFileProxy :AbstractNotifyPropertyChanged, IEquatable<RecentFileProxy>
{
    private readonly RecentFile _recentFile;
    private string _label;
    public string Name => _recentFile.Name;
    public string OpenToolTip => $"Open {_recentFile.Name}";
    public string RemoveToolTip => $"Remove {_recentFile.Name} from recent list";
    public DateTime Timestamp => _recentFile.Timestamp;
    public ICommand OpenCommand { get; }
    public ICommand RemoveCommand { get; }

    public string Label
    {
        get => _label;
        set => SetAndRaise(ref _label, value);
    }

    public RecentFileProxy(RecentFile recentFile,
        Action<RecentFile> openAction,
        Action<RecentFile> removeAction)
    {
        _recentFile = recentFile;

        OpenCommand = new RelayCommand(() => openAction(recentFile));
        RemoveCommand = new RelayCommand(() => removeAction(recentFile));
    }

    public override string ToString()
    {
        return $"{Name}";
    }

    #region Equality

    public bool Equals(RecentFileProxy other)
    {
        if(other is null) return false;
        if(ReferenceEquals(this, other)) return true;
        return Equals(_recentFile, other._recentFile);
    }

    public override bool Equals(object obj)
    {
        if(obj is null) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        return Equals((RecentFileProxy) obj);
    }

    public override int GetHashCode()
    {
        return _recentFile?.GetHashCode() ?? 0;
    }

    public static bool operator ==(RecentFileProxy left, RecentFileProxy right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RecentFileProxy left, RecentFileProxy right)
    {
        return !Equals(left, right);
    }

    #endregion
}