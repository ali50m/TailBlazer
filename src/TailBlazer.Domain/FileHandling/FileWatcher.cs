using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TailBlazer.Domain.Annotations;
using TailBlazer.Domain.Ratings;

namespace TailBlazer.Domain.FileHandling;

public class FileWatcher :IFileWatcher
{
    private readonly ISubject<long> _scanFrom = new BehaviorSubject<long>(0);

    private FileInfo FileInfo { get; }

    public string Extension => FileInfo.Extension;

    public FileWatcher([NotNull] FileInfo fileInfo, IRatingService ratingsMetrics, IScheduler scheduler = null)
    {
        FileInfo = fileInfo;
        if(fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));

        scheduler = scheduler ?? Scheduler.Default;

        var refreshRate = ratingsMetrics.Metrics.Take(1)
            .Select(metrics => TimeSpan.FromMilliseconds(metrics.RefreshRate))
            .Wait();

        var shared = _scanFrom.Select(start => start == 0
                ? fileInfo.WatchFile(scheduler: scheduler, refreshPeriod: refreshRate)
                : fileInfo.WatchFile(scheduler: scheduler, refreshPeriod: refreshRate).ScanFromEnd())
            .Switch();

        Latest = shared
            .TakeWhile(notification => notification.Exists).Repeat()
            .Replay(1).RefCount();

        Status = fileInfo.WatchFile(scheduler: scheduler).Select(notificiation =>
            {
                if(!notificiation.Exists || notificiation.Error != null)
                    return FileStatus.Error;

                return FileStatus.Loaded;
            })
            .StartWith(FileStatus.Loading)
            .DistinctUntilChanged();
    }

    public IObservable<FileStatus> Status { get; }

    public IObservable<FileNotification> Latest { get; }

    public string FullName => FileInfo.FullName;

    public string Name => FileInfo.Name;

    public string Folder => FileInfo.DirectoryName;

    public void ScanFrom(long scanFrom)
    {
        _scanFrom.OnNext(scanFrom);
    }

    public void Clear()
    {
        _scanFrom.OnNext(FileInfo.Length);
    }

    public void Reset()
    {
        _scanFrom.OnNext(0);
    }
}