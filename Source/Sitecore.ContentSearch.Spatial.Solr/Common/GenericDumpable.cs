using System.IO;
using Sitecore.ContentSearch.Linq.Common;

namespace Sitecore.ContentSearch.Spatial.Solr.Common
{
    public class GenericDumpable : IDumpable
    {
        public GenericDumpable(object value)
        {
            Value = value;
        }

        protected object Value { get; set; }

        public virtual void WriteTo(TextWriter writer)
        {
            var dumpable = Value as IDumpable;
            if (dumpable != null)
            {
                dumpable.WriteTo(writer);
            }
            else
            {
                writer.WriteLine(Value);
            }
        }
    }
}