﻿using System.Collections.Generic;
using Sitecore.ContentSearch.Linq.Nodes;

namespace Sitecore.ContentSearch.Spatial.Solr.Nodes
{
    public class WithinRadiusNode : QueryNode
    {
        public WithinRadiusNode(QueryNode sourceNode, string field, double lat, double lon, int radius)
        {
            SourceNode = sourceNode;
            Field = field;
            Lat = lat;
            Lon = lon;
            Radius = radius;
        }

        public QueryNode SourceNode { get; protected set; }
        public string Field { get; protected set; }
        public double Lat { get; protected set; }
        public double Lon { get; protected set; }
        public int Radius { get; protected set; }

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