using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Spatial.DataTypes;

namespace Sitecore.ContentSearch.Spatial.Solr
{
    public static class SearchExtensions
    {
        public static IQueryable<TSource> WithinRadius<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, double lat, double lon, int radius)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var exp = Expression.Call(null,
                ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource), typeof(TKey)),
                source.Expression, Expression.Quote(keySelector), Expression.Constant(lat, typeof(double)),
                Expression.Constant(lon, typeof(double)), Expression.Constant(radius, typeof(int)));
            return source.Provider.CreateQuery<TSource>(exp);
        }

        public static IQueryable<TSource> OrderByNearest<TSource>(this IQueryable<TSource> source) where  TSource : SearchResultItem
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return source.OrderBy(i => i["score"]);
        }

        public static IQueryable<TSource> OrderByDistance<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, SpatialPoint>> keySelector, SpatialPoint referencePoint)
        {
            return source.OrderByDistance(keySelector, referencePoint.Lat, referencePoint.Lon);
        }

        public static IQueryable<TSource> OrderByDistance<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, SpatialPoint>> keySelector, double lat, double lon)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var expression = Expression.Call(null,
                ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource)), source.Expression,
                Expression.Quote(keySelector), Expression.Constant(lat, typeof(double)),
                Expression.Constant(lon, typeof(double)));

            return source.Provider.CreateQuery<TSource>(expression);
        }
    }
}