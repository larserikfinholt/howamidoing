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
            vm.Graph = new GraphViewModel();// gp.ProcessGraph(goal);
            //var timeFirstPoint = GraphProcessor.GetDateOfLastOrFirstGraphPoint(true, goal, _now);

            decimal currentLevel = 0;
            DateTime? previousDoneItTime = null;
            decimal perHour = GetHourlyDecreaseRate(goal);
            var cutoffLevel = goal.GetLowerCutoff();

            if (goal.DoneIts.Count > 0)
            {
                foreach (var done in goal.DoneIts.OrderBy(d => d.Date))
                {
                    var oldCurrentLevel = currentLevel;
                    if (previousDoneItTime == null)
                    {
                        // First doneit starts counting
                        currentLevel = done.Amount;
                        vm.Graph.AddPoint(done, oldCurrentLevel, currentLevel);
                    }
                    else
                    {
                        // Calculate current level
                        var dif = (done.Date - (DateTime)previousDoneItTime).TotalHours;
                        oldCurrentLevel = currentLevel - (perHour * Convert.ToDecimal(dif));
                        if (oldCurrentLevel < cutoffLevel)
                        {
                            // Add extra point at lower threshold
                            var cutoffTime = CalculateLowerCutoffTime(goal, oldCurrentLevel, previousDoneItTime.Value);

                            vm.Graph.Points.Add(new GraphPoint { Time = cutoffTime, Amount = 0, y = cutoffLevel });
                            oldCurrentLevel = cutoffLevel;
                        }

                        currentLevel = oldCurrentLevel + done.Amount;
                        vm.Graph.AddPoint(done, oldCurrentLevel, currentLevel);
                    }
                    previousDoneItTime = done.Date;
                }
                // Calculate current level
                var diff = (_now - (DateTime)previousDoneItTime).TotalHours;
                currentLevel -= (perHour * Convert.ToDecimal(diff));

                // Check for cutoff
                if (currentLevel < cutoffLevel)
                {
                    currentLevel = cutoffLevel;
                    vm.Cutoff = CutoffStatus.Active;
                }

                // Add current level
                vm.Graph.Points.Add(new GraphPoint { Time = _now, Amount = 0, y = currentLevel });


                vm.Status = currentLevel < 0 ? GoalStatus.Behind : GoalStatus.OnTrack;

                vm.AtZero = TimeSpan.FromHours(Convert.ToDouble(currentLevel / perHour));
                vm.CurrentLevel = currentLevel;
            }
            else
            {
                vm.Graph = GraphProcessor.CreateEmptyGraph(goal, _now);
            }

                var graph = vm.Graph;
                var first = graph.Points.OrderBy(x => x.Time).First().Time;
                var last = graph.Points.OrderBy(x => x.Time).Last().Time;
                var totalSecs = ((last - first).TotalSeconds);
                if (totalSecs > 0)
                {
                    foreach (var point in graph.Points)
                    {
                        point.x = Convert.ToDecimal((point.Time - first).TotalSeconds / totalSecs);
                    }
                }


            //GraphProcessor.CalculateXPositions(out vm);

            return vm;
        }

        public DateTime CalculateLowerCutoffTime(Goal goal, decimal oldCurrentLevel, DateTime previousDoneItTime)
        {
            var hourdecr = GoalProcessor.GetHourlyDecreaseRate(goal);
            var lowerCutoff = goal.GetLowerCutoff();
            var hours = (oldCurrentLevel - lowerCutoff) / hourdecr;
            return previousDoneItTime.AddHours(Convert.ToDouble(hours));
        }

        public static decimal GetHourlyDecreaseRate(Goal goal)
        {
            decimal perHour = 0;
            switch (goal.IntervalType)
            {
                case IntervalType.Dayly:
                    perHour = (decimal)goal.Amount / 24;
                    break;
                case IntervalType.Weekly:
                    // Number per hour
                    perHour = (decimal)goal.Amount / (7 * 24);
                    break;
                case IntervalType.Monthly:
                    perHour = (decimal)goal.Amount / (30 * 24);
                    break;
                default:
                    new NotSupportedException();
                    break;
            }
            return perHour;
        }



        public GoalStatus FindOverallStatus(List<GoalViewModel> goals)
        {
            if (goals == null || goals.Count < 1) return GoalStatus.NoGoalsDefined;

            var g = goals.OrderByDescending(x => x.Status).First();

            return g == null ? GoalStatus.NoGoalsDefined : g.Status;
        }


    }
}