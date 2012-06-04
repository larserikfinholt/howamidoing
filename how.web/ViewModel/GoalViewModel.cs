using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using how.web.Models;

namespace how.web.ViewModel
{
    public class GoalViewModel
    {
        public Goal Goal { get; set; }
        public GoalStatus Status { get; set; }
        public TimeSpan AtZero { get; set; }

        public decimal CurrentLevel { get; set; }
        public CutoffStatus Cutoff { get; set; }

        public GraphViewModel Graph { get; set; }

        public string SerializePoints()
        {
            var q = (from p in Graph.Points select new []{p.x, p.y}).ToArray();
			return	ServiceStack.Text.JsonSerializer.SerializeToString<decimal[][]>(q);
        }

    }
    public enum CutoffStatus
    {
        None,
        Active
    }
}
