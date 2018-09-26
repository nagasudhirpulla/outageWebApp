using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutageDataLayer.QueryResultModels
{
    public class OutageQueryResult
    {
        public int ElementId { get; set; }
        public string ElementName { get; set; }
        public string ElementType { get; set; }
        public string OutageType { get; set; }
        public DateTime OutTime { get; set; }
        public DateTime InTime { get; set; }
        public string VoltageLevel { get; set; }
        public string OutageReason { get; set; }
        public string OutageCategory { get; set; }
        public string ElementOwner { get; set; }
    }
}
