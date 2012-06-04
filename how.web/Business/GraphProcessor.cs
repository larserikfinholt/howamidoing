using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using how.web.Models;
using how.web.ViewModel;

namespace how.web.Business
{
    public class GraphProcessor
    {
        private DateTime _now;
        public GraphProcessor(DateTime now)
        {
            _now = now;
        }

        public static DateTime GetDateOfLastOrFirstGraphPoint(bool isFirst, Goal goal, DateTime now)
        {
            if (isFirst)
            {
                return now.AddMonths(-4);
                
            }
            return now;

            var a = isFirst ? -1 : 1;
            if (goal.EvaluationType == GoalEvaluation.Countinious)
            {
                switch (goal.IntervalType)
                {
                    case IntervalType.Dayly:
                        return now.AddDays(2*a);
                    case IntervalType.Weekly:
                        return now.AddDays(7*a);
                    case IntervalType.Monthly:
                        return now.AddMonths(1*a);
                    default:
                        throw new ApplicationException("Unknown Intervaltype");
                }
            }
            else
            {
                if (isFirst)
                {
                    return goal.StartDate.HasValue ? goal.StartDate.Value : now.AddDays(-10);
                }
                else
                {
                    return goal.EndDate.HasValue ? goal.EndDate.Value : now.AddDays(10);
                }
            }
        }


        public GraphViewModel ProcessGraph(Goal goal)
        {
            decimal currentLevel = 0;
            DateTime? previousDoneItTime = null;
            decimal perHour = GoalProcessor.GetHourlyDecreaseRate(goal);

            var gvm = GraphProcessor.CreateEmptyGraph(goal,_now);
            

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
                    gvm.Points.Add(new GraphPoint { Amount = done.Amount, Time = done.Date });
                }
                var diff = (_now - (DateTime)previousDoneItTime).TotalHours;
                currentLevel -= (perHour * Convert.ToDecimal(diff));
                //vm.Status = currentLevel < 0 ? GoalStatus.Behind : GoalStatus.OnTrack;

                //vm.AtZero = TimeSpan.FromHours(Convert.ToDouble(currentLevel / perHour));
                //vm.CurrentLevel = currentLevel;
            }
            return gvm;
        }
        public static GraphViewModel CreateEmptyGraph(Goal goal, DateTime now)
        {
            var vm = new GraphViewModel();
            if (goal.EvaluationType == GoalEvaluation.Countinious)
            {
                var p1 = new GraphPoint { Amount = 0 };
                var p2 = new GraphPoint { Amount = 0 };
                switch (goal.IntervalType)
                {
                    case IntervalType.Dayly:
                        p1.Time = now.AddDays(-2);
                        p2.Time = now.AddDays(2);
                        break;
                    case IntervalType.Weekly:
                        p1.Time = now.AddDays(-14);
                        p2.Time = now.AddDays(7);

                        break;
                    case IntervalType.Monthly:
                        p1.Time = now.AddMonths(-2);
                        p2.Time = now.AddMonths(1);
                        break;
                    default:
                        break;
                }
                vm.Points = new List<GraphPoint> { p1, p2 };

            }
            else
            {
                var p1 = new GraphPoint { Amount = 0, Time=goal.StartDate.HasValue?goal.StartDate.Value:now };
                var p2 = new GraphPoint { Amount = 0 , Time=goal.EndDate.HasValue?goal.EndDate.Value:now};
                vm.Points = new List<GraphPoint> { p1, p2 };
            }
            return vm;
        }

        //public  static void CalculateXPositions( out GoalViewModel goal)

        //{
        //    var graph = goal.Graph;
        //    var first = graph.Points.OrderBy(x => x.Time).First().Time;
        //    var last = graph.Points.OrderBy(x => x.Time).Last().Time;
        //    var totalSecs = (first - last).TotalSeconds;
        //    foreach (var point in graph.Points)
        //    {
        //        point.x = 0;
                
        //    }
            
        //}
    }
}