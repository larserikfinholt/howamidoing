using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using how.web.Models;
using how.web.ViewModel;

namespace how.web.Business
{
    public class GoalProcessor
    {
        private DateTime _now;
        public GoalProcessor()
        {
            _now = DateTime.Now;
        }
        public GoalProcessor(DateTime now)
        {
            _now = now;
        }

        public GoalViewModel ProcessGoal(Goal goal)
        {
            var vm = new GoalViewModel();
            vm.Status = GoalStatus.NotStarted;
            vm.Goal = goal;

            // Goal: 4 hours / Week

            decimal perHour = 0;
            decimal currentLevel = 0;
            DateTime? previousDoneItTime = null;

            switch (goal.IntervalType)
            {
                case IntervalType.Dayly:
                    perHour = (decimal)goal.Interval / 24;
                    break;
                case IntervalType.Weekly:
                    // Number per hour
                    perHour = (decimal)goal.Interval / (7 * 24);
                    break;
                case IntervalType.Monthly:
                    perHour = (decimal)goal.Interval / (30 * 24);
                    break;
                default:
                    new NotSupportedException();
                    break;
            }
            if (goal.DoneIts.Count > 0)
            {
                foreach (var done in goal.DoneIts.OrderBy(d => d.Date))
                {
                    if (previousDoneItTime == null)
                    {
                        previousDoneItTime = done.Date;
                        currentLevel = done.Amount;
                    }
                    else
                    {
                        currentLevel += done.Amount;
                    }

                }
                var diff = (_now - (DateTime)previousDoneItTime).TotalHours;
                currentLevel -= (perHour * Convert.ToDecimal(diff));
                vm.Status = currentLevel < 0 ? GoalStatus.Behind : GoalStatus.OnTrack;
                
            }

            
            return vm;
        }


        public GoalStatus FindOverallStatus(List<GoalViewModel> goals)
        {
            if (goals == null || goals.Count < 1) return GoalStatus.NoGoalsDefined;

            var g = goals.OrderByDescending(x => x.Status).First();

            return g == null ? GoalStatus.NoGoalsDefined : g.Status;
        }
    }
}