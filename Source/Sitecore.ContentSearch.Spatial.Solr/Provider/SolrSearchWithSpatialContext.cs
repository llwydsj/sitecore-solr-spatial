using System.Linq;
using Sitecore.ContentSearch.Abstractions;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Pipelines.QueryGlobalFilters;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using Sitecore.ContentSearch.SolrProvider;
using Sitecore.ContentSearch.Spatial.Solr.Indexing;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Diagnostics;

namespace Sitecore.ContentSearch.Spatial.Solr.Provider
{
    public class SolrSearchWithSpatialContext : SolrSearchContext, IProviderSearchContext
    {

        private readonly SolrSearchIndex index;
        private readonly SearchSecurityOptions securityOptions;
        private readonly IContentSearchConfigurationSettings contentSearchSettings;
        private ISettings settings;


        public SolrSearchWithSpatialContext(SolrSearchIndex index, SearchSecurityOptions options = SearchSecurityOptions.EnableSecurityCheck)
            : base(index, options)
        {
            Assert.ArgumentNotNull(index, "index");
            Assert.ArgumentNotNull(options, "options");
            this.index = index;
            contentSearchSettings = this.index.Locator.GetInstance<IContentSearchConfigurationSettings>();
            settings = this.index.Locator.GetInstance<ISettings>();
            securityOptions = options;
        }

        public new IQueryable<TItem> GetQueryable<TItem>()
        {
            return GetQueryable<TItem>(new IExecutionContext[0]);
        }

        public new IQueryable<TItem> GetQueryable<TItem>(IExecutionContext executionContext)
        {
            return GetQueryable<TItem>(new IExecutionContext[1]
              {
                executionContext
              });
        }

        public new IQueryable<TItem> GetQueryable<TItem>(params IExecutionContext[] executionContexts)
        {
            var linqToSolrIndex = new LinqToSolrIndexWithSpatial<TItem>(this, executionContexts);
            if (contentSearchSettings.EnableSearchDebug())
                ((IHasTraceWriter)linqToSolrIndex).TraceWriter = new LoggingTraceWriter(SearchLog.Log);

            var queryable = linqToSolrIndex.GetQueryable();
            if (typeof(TItem).IsAssignableFrom(typeof(SearchResultItem)))
            {
                var globalFiltersArgs = new QueryGlobalFiltersArgs(linqToSolrIndex.GetQueryable(), typeof(TItem), executionContexts.ToList());
                Index.Locator.GetInstance<ICorePipeline>().Run("contentSearch.getGlobalLinqFilters", globalFiltersArgs);
                queryable = (IQueryable<TItem>)globalFiltersArgs.Query;
            }
            return queryable;
        }

    }
}
