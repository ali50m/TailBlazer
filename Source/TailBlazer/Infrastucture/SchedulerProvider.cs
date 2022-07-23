using System.Reactive.Concurrency;
using System.Windows.Threading;
using TailBlazer.Domain.Infrastructure;

namespace TailBlazer.Infrastucture
{
    public class SchedulerProvider :ISchedulerProvider
    {
        public SchedulerProvider(Dispatcher dispatcher)
        {
            //MainThread = new DispatcherScheduler(dispatcher);
            MainThread = new SynchronizationContextScheduler(new DispatcherSynchronizationContext(dispatcher));
        }

        public IScheduler MainThread { get; }

        public IScheduler Background { get; } = TaskPoolScheduler.Default;
    }
}