using System.Collections.Generic;
using System.Linq;

namespace Narik.Common.Shared.Models
{
    public class Change<T>
        where T : class
    {
        public List<T> Modified { get; set; }
        public List<T> Added { get; set; }
        public List<T> Removed { get; set; }
        public List<T> NotModified { get; set; }

        public Change<TDest> OfType<TDest>() where TDest : class
        {
            return new Change<TDest>
            {
                Modified = Modified?.OfType<TDest>().ToList(),
                Added = Added?.OfType<TDest>().ToList(),
                Removed = Removed?.OfType<TDest>().ToList(),
                NotModified = NotModified?.OfType<TDest>().ToList(),
            };
        }
    }
}
