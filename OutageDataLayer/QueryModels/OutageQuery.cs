using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutageDataLayer.QueryModels
{
    public class OutageQuery
    {
        public int? ElementId { get; set; }
        public DateTime? OutFromDate { get; set; }
        public DateTime? OutToDate { get; set; }
        public DateTime? InFromDate { get; set; }
        public DateTime? InToDate { get; set; }
        public string OutageType { get; set; }
        public string ElementType { get; set; }
        public string ReasonSearchType { get; set; }
        public string ReasonSearchText { get; set; }
    }
}
