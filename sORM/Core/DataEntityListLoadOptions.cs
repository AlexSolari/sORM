using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core
{
    /// <summary>
    /// Pagination options
    /// </summary>
    public class DataEntityListLoadOptions
    {
        /// <summary>
        /// Creates pagination options.
        /// </summary>
        /// <param name="size">Page size.</param>
        /// <param name="index">Page index.</param>
        /// <param name="by">Field name to order by.</param>
        /// <param name="asc">Is sorting in ascending order.</param>
        public DataEntityListLoadOptions(int size = 10, int index = 0, string by = null, bool asc = false)
        {
            PageSize = size;
            PageNumber = index;
            OrderBy = by;
            asc = false;
        }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string OrderBy { get; set; }
        public bool OrderAsc { get; set; }
    }
}
