using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace how.web.ViewModel
{
    public class GraphViewModel
    {
        public List<GraphPoint> Points { get; set; }
    }
    public class GraphPoint
    {
        public decimal x { get; set; }
        public decimal y { get; set; }
        public DateTime Time { get; set; }
        public decimal Amount { get; set; }
    }
}