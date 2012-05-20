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
        public DateTime AtZero { get; set; }
    }
}
