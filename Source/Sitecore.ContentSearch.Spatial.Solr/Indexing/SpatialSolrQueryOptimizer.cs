using Sitecore.ContentSearch.Linq.Nodes;
using Sitecore.ContentSearch.Linq.Solr;
using Sitecore.ContentSearch.Spatial.Solr.Nodes;

namespace Sitecore.ContentSearch.Spatial.Solr.Indexing
{
    public class SpatialSolrQueryOptimizer : SolrQueryOptimizer
    {
        protected override QueryNode Visit(QueryNode node, SolrQueryOptimizerState state)
        {
            if (node.NodeType == QueryNodeType.Custom)
            {
                if (node is WithinRadiusNode)
                {
                    return VisitWithinRadius((WithinRadiusNode)node, state);
                }

                if (node is OrderByDistanceNode)
                {
                    return VisitOrderByDistance((OrderByDistanceNode) node, state);
                }
            }
           
            return base.Visit(node, state);
        }

        private QueryNode VisitWithinRadius(WithinRadiusNode radiusNode, SolrQueryOptimizerState state)
        {
            return new WithinRadiusNode(this.Visit(radiusNode.SourceNode, state), radiusNode.Field, radiusNode.Lat, radiusNode.Lon, radiusNode.Radius);
        }

        private QueryNode VisitOrderByDistance(OrderByDistanceNode orderByNode, SolrQueryOptimizerState state)
        {
            return new OrderByDistanceNode(this.Visit(orderByNode.SourceNode, state), orderByNode.Field, orderByNode.Lat, orderByNode.Lon);
        }
    }
}