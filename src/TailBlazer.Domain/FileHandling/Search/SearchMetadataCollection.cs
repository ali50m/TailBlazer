using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Kernel;
using TailBlazer.Domain.Annotations;
using TailBlazer.Domain.Infrastructure;

namespace TailBlazer.Domain.FileHandling.Search;

public sealed class SearchMetadataCollection :ISearchMetadataCollection
{
    private readonly IDisposable _cleanUp;
    private readonly ILogger _logger;

    private readonly ISourceCache<SearchMetadata, string> _searches =
        new SourceCache<SearchMetadata, string>(t => t.SearchText);

    public SearchMetadataCollection([NotNull] ILogger logger)
    {
        if(logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger;
        Metadata = _searches.AsObservableCache();

        _cleanUp = new CompositeDisposable(_searches, Metadata);
    }

    public IObservableCache<SearchMetadata, string> Metadata { get; }

    public int NextIndex()
    {
        if(_searches.Count == 0)
            return 0;

        return _searches.Items.Select(m => m.Position).Max() + 1;
    }

    public void Add([NotNull] IEnumerable<SearchMetadata> metadata)
    {
        if(metadata == null) throw new ArgumentNullException(nameof(metadata));
        var searchMetadatas = metadata.AsArray();
        _searches.AddOrUpdate(searchMetadatas);
        _logger.Info("{0} SearchMetadata has been loaded", searchMetadatas.Count());
    }

    public void AddorUpdate([NotNull] SearchMetadata metadata)
    {
        if(metadata == null) throw new ArgumentNullException(nameof(metadata));
        _searches.AddOrUpdate(metadata);
        _logger.Info("Search metadata has changed: {0}", metadata);
    }

    public void Remove(string searchText)
    {
        _searches.Remove(searchText);
        _logger.Info("Search metadata has been removed: {0}", searchText);
    }

    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}