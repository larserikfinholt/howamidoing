using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using how.web.Models;

namespace how.web.ViewModel
{
    public enum GoalStatus
    {
        NoGoalsDefined,
        NotStarted,
        OnTrack,
        AlmostBehind,
        Behind
    }
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Goals = new List<GoalViewModel>();
        }
        public string UserName { get; set; }
        public GoalStatus OverallStatus { get; set; }
        public List<GoalViewModel> Goals { get; set; }
    }
}