using Sitecore.ContentSearch.Linq.Methods;
using Sitecore.ContentSearch.Linq.Nodes;
using Sitecore.ContentSearch.Linq.Solr;
using Sitecore.ContentSearch.Spatial.Solr.Nodes;
using SolrNet;

namespace Sitecore.ContentSearch.Spatial.Solr.Indexing
{
    public class SolrSpatialQueryMapper : SolrQueryMapper
    {
        public SolrSpatialQueryMapper(SolrIndexParameters parameters) : base(parameters)
        {
        }

        protected override AbstractSolrQuery Visit(QueryNode node, SolrQueryMapperState state)
        {
            if (node.NodeType == QueryNodeType.Custom)
            {
                if (node is WithinRadiusNode)
                {
                    return VisitWithinRadius((WithinRadiusNode)node, state);
                }
                if (node is OrderByDistanceNode)
                {
                    return StripOrderByDistance((OrderByDistanceNode)node, state);
                }
            }
            return base.Visit(node, state);
        }

        protected virtual AbstractSolrQuery VisitWithinRadius(WithinRadiusNode radiusNode, SolrQueryMapper.SolrQueryMapperState state)
        {
            var orignialQuery = this.Visit(radiusNode.SourceNode, state);
            var spatialQuery = new SolrQuery($"{{!geofilt pt={radiusNode.Lat},{radiusNode.Lon} sfield={radiusNode.Field} d={radiusNode.Radius} score=distance}}");
            var combinedQuery = orignialQuery && spatialQuery;
            return combinedQuery;
        }

        protected virtual AbstractSolrQuery StripOrderByDistance(OrderByDistanceNode node, SolrQueryMapperState state)
        {
            var orderingMethod = new OrderByMethod($"geodist({node.Field},{node.Lat},{node.Lon})", node.FieldType, node.SortDirection);
            state.AdditionalQueryMethods.Add(orderingMethod);
            return base.Visit(node.SourceNode, state);
        }
    }
}