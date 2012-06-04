using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using how.web.Models;

namespace how.web.ViewModel
{
    public class GraphViewModel
    {
        public GraphViewModel() { Points = new List<GraphPoint>(); }
        public List<GraphPoint> Points { get; set; }

        public void AddPoint(DoneIt doneit, decimal before, decimal after)
        {
            Debug.WriteLine(" - adding {0} and {1} at {2}", before, after, doneit.Date.ToShortDateString());
            // Add point just before
            Points.Add(new GraphPoint { Time = doneit.Date, Amount = 0, y = before });
            // Add point after
            Points.Add(new GraphPoint { Time = doneit.Date.AddSeconds(1), Amount = doneit.Amount, y = after });
        }
        
    }
    public class GraphPoint
    {
        public decimal x { get; set; }
        public decimal y { get; set; }
        public DateTime Time { get; set; }
        public decimal Amount { get; set; }
    }
}