using System;
using System.Linq.Expressions;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Nodes;
using Sitecore.ContentSearch.Linq.Parsing;
using Sitecore.ContentSearch.Spatial.Solr.Nodes;

namespace Sitecore.ContentSearch.Spatial.Solr.Parsing
{
    public class ExtendedExpressionParser : ExpressionParser
    {
        public ExtendedExpressionParser(Type elementType, Type itemType, FieldNameTranslator fieldNameTranslator) : base(elementType, itemType, fieldNameTranslator)
        {
        }

        protected override QueryNode VisitMethodCall(MethodCallExpression methodCall)
        {
            if (methodCall.Method.DeclaringType == typeof (SearchExtensions))
            {
                return this.VisitQueryableExtensionMethod(methodCall);
            }
            return base.VisitMethodCall(methodCall);
        }

        protected override QueryNode VisitQueryableExtensionMethod(MethodCallExpression methodCall)
        {
            switch (methodCall.Method.Name)
            {
                case "WithinRadius":
                    return this.VisitWithinRadiusMethod(methodCall);
                case "OrderByDistance":
                    return this.VisitOrderByDistanceMethod(methodCall);
            }
            return base.VisitQueryableExtensionMethod(methodCall);
        }

        protected virtual QueryNode VisitWithinRadiusMethod(MethodCallExpression methodCall)
        {
            QueryNode sourceNode = this.Visit(GetArgument(methodCall.Arguments, 0));
            var latExpression = (ConstantExpression)GetArgument(methodCall.Arguments, 2);
            var lonExpression = (ConstantExpression)GetArgument(methodCall.Arguments, 3);
            var distanceExpression = (ConstantExpression)GetArgument(methodCall.Arguments, 4);

            var lat = (double)latExpression.Value;
            var lon = (double)lonExpression.Value;
            var radius = (int)distanceExpression.Value;
            var lambdaExpression = Convert<LambdaExpression>(StripQuotes(GetArgument(methodCall.Arguments, 1)));
            if (lambdaExpression.Body.NodeType == ExpressionType.MemberAccess)
            {
                QueryNode queryNode = Visit(lambdaExpression.Body);
                var fieldNode = queryNode as FieldNode;
                
                if (fieldNode != null)
                    return new WithinRadiusNode(sourceNode, fieldNode.FieldKey, lat, lon, radius);
                
                throw new NotSupportedException(string.Format("Faceting can only be done on '{0}'. Expression used '{1}'", typeof(FieldNode).FullName, methodCall.Arguments[1].Type.FullName));
            }
            else
            {
                var fieldNode = Visit(lambdaExpression.Body) as FieldNode;
                if (fieldNode == null)
                    throw new NotSupportedException(string.Format("Ordering can only be done on '{0}'. Expression used '{1}'", typeof(FieldNode).FullName, methodCall.Arguments[1].Type.FullName));

                return new WithinRadiusNode(sourceNode, fieldNode.FieldKey, lat, lon, radius);
            }
        }

        protected virtual QueryNode VisitOrderByDistanceMethod(MethodCallExpression methodCall)
        {
            var sourceNode = this.Visit(GetArgument(methodCall.Arguments, 0));
            var lambdaExpression = Convert<LambdaExpression>(StripQuotes(GetArgument(methodCall.Arguments, 1)));
            var latExpression = (ConstantExpression) GetArgument(methodCall.Arguments, 2);
            var lonExpression = (ConstantExpression) GetArgument(methodCall.Arguments, 3);
            var lat = (double) latExpression.Value;
            var lon = (double) lonExpression.Value;

            if (lambdaExpression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var queryNode = Visit(lambdaExpression.Body);
                var fieldNode = queryNode as FieldNode;

                if (fieldNode != null)
                {
                    return new OrderByDistanceNode(sourceNode, fieldNode.FieldKey, lat, lon);
                }
            }
            
            return base.VisitOrderByMethod(methodCall);
        }
    }
}