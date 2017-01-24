using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Nodes;
using System.Collections.Generic;

namespace Sitecore.ContentSearch.Spatial.Solr.Nodes
{
    public class OrderByDistanceNode : OrderByNode
    {
        public OrderByDistanceNode(QueryNode sourceNode, string field, double lat, double lon)
            : base(sourceNode, field, typeof(double), SortDirection.Ascending)
        {
            Lat = lat;
            Lon = lon;
        }

        public double Lat { get; protected set; }
        public double Lon { get; protected set; }

        public override QueryNodeType NodeType => QueryNodeType.Custom;

        public override IEnumerable<QueryNode> SubNodes
        {
            get
            {
                yield return SourceNode;
            }
        }
    }
}