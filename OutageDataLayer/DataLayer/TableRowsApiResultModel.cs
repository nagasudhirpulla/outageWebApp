using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutageDataLayer.DataLayer
{
    public class TableRowsApiResultModel
    {
        public List<string> TableColNames { get; set; } = new List<string>();
        public List<string> TableColTypes { get; set; } = new List<string>();
        public List<List<object>> TableRows { get; set; } = new List<List<object>>();
    }
}
