﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TailBlazer.Domain.Annotations;
using TailBlazer.Domain.Formatting;
using TailBlazer.Domain.Infrastructure;
using TailBlazer.Domain.Settings;

namespace TailBlazer.Views.Tail;

public class GeneralOptionBindings :IDisposable
{
    private readonly IDisposable _cleanUp;
    public IProperty<bool> HighlightTail { get; }
    public IProperty<bool> UsingDarkTheme { get; }
    public IProperty<bool> ShowLineNumbers { get; }

    public GeneralOptionBindings([NotNull] ISetting<GeneralOptions> generalOptions,
        ISchedulerProvider schedulerProvider)
    {
        UsingDarkTheme = generalOptions.Value
            .ObserveOn(schedulerProvider.MainThread)
            .Select(options => options.Theme == Theme.Dark)
            .ForBinding();

        HighlightTail = generalOptions.Value
            .ObserveOn(schedulerProvider.MainThread)
            .Select(options => options.HighlightTail)
            .ForBinding();

        ShowLineNumbers = generalOptions.Value
            .ObserveOn(schedulerProvider.MainThread)
            .Select(options => options.ShowLineNumbers)
            .ForBinding();

        _cleanUp = new CompositeDisposable(UsingDarkTheme, HighlightTail, ShowLineNumbers);
    }

    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}