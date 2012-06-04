using System;
using System.Collections.Generic;
using System.Linq;
using how.web.Business;
using how.web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace how.test
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void it_should_start_one_interval_before_and_end_one_interval_after_if_interval()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { EvaluationType = GoalEvaluation.Countinious, IntervalType = IntervalType.Weekly };

            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(vm.Graph.Points[0].Time, now.AddDays(-14));
            Assert.AreEqual(vm.Graph.Points[1].Time, now.AddDays(7));
            Assert.AreEqual(0, vm.Graph.Points[0].Amount);
            Assert.AreEqual(0, vm.Graph.Points[1].Amount);

            goal = new Goal() { EvaluationType = GoalEvaluation.Countinious, IntervalType = IntervalType.Monthly };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(vm.Graph.Points[0].Time, now.AddMonths(-2));
            Assert.AreEqual(vm.Graph.Points[1].Time, now.AddMonths(1));

            goal = new Goal() { EvaluationType = GoalEvaluation.Countinious, IntervalType = IntervalType.Dayly };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(vm.Graph.Points[0].Time, now.AddDays(-2));
            Assert.AreEqual(vm.Graph.Points[1].Time, now.AddDays(2));

        }
        [TestMethod]
        public void it_should_start_and_end_on_specified_dates_if_timelimited()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { EvaluationType = GoalEvaluation.Timelimited };

            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(0, vm.Graph.Points[0].Amount);
            Assert.AreEqual(0, vm.Graph.Points[1].Amount);

            goal = new Goal() { EvaluationType = GoalEvaluation.Timelimited, StartDate = now.AddDays(-10), EndDate = now.AddDays(10) };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(vm.Graph.Points[0].Time, now.AddDays(-10));
            Assert.AreEqual(vm.Graph.Points[1].Time, now.AddDays(10));

            


        }
        [TestMethod]
        public void it_should_set_value_of_first_point_to_the_amount_the_first_doneit()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { Amount = 7, IntervalType = IntervalType.Weekly };

            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 10, Date = now.AddDays(-2) } };
            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(10, vm.Graph.Points.Where(p => p.Time == now.AddDays(-2).AddSeconds(1)).FirstOrDefault().Amount);

        }
        [TestMethod]
        public void it_should_build_the_graph_correct()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { Amount = 7, IntervalType = IntervalType.Weekly };

            // Simple one entry
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 7, Date = now.AddDays(-7) } };
            var vm = target.ProcessGoal(goal);
            AssertPointOnGraph(vm, 0, 0, now.AddDays(-7));
            AssertPointOnGraph(vm, 1, 7, now.AddDays(-7));
            AssertPointOnGraph(vm, 2, 0, now.AddDays(-7));

            // Strait line from 7 to 0, with on extra point
            goal.DoneIts = new List<DoneIt>() { 
                new DoneIt() { Amount = 7, Date = now.AddDays(-7) },
                new DoneIt() { Amount = 0, Date = now.AddDays(-3) },
            };
            vm = target.ProcessGoal(goal);
            AssertPointOnGraph(vm, 0, 0, now.AddDays(-7));
            AssertPointOnGraph(vm, 1, 7, now.AddDays(-7));
            AssertPointOnGraph(vm, 2, 3, now.AddDays(-4));
            AssertPointOnGraph(vm, 3, 3, now.AddDays(-4));
            AssertPointOnGraph(vm, 4, 0, now.AddDays(0));

            // Strait line from 7 to 0, with on extra point adding one
            goal.DoneIts = new List<DoneIt>() { 
                new DoneIt() { Amount = 7, Date = now.AddDays(-7) },
                new DoneIt() { Amount = 1, Date = now.AddDays(-3) },
            };
            vm = target.ProcessGoal(goal);
            AssertPointOnGraph(vm, 0, 0, now.AddDays(-7));
            AssertPointOnGraph(vm, 1, 7, now.AddDays(-7));
            AssertPointOnGraph(vm, 2, 3, now.AddDays(-4));
            AssertPointOnGraph(vm, 3, 4, now.AddDays(-4));
            AssertPointOnGraph(vm, 4, 1, now.AddDays(0));
        }

        private static void AssertPointOnGraph(web.ViewModel.GoalViewModel vm, int pointNo, decimal value, DateTime time)
        {
            var point = vm.Graph.Points.OrderBy(x => x.Time).ToList()[pointNo];
            Assert.IsTrue(Math.Abs(value - point.y) < (decimal)0.0001, string.Format("Diff should be zero, got {0}", value-point.y));
            Assert.IsTrue((time-point.Time).TotalSeconds<2, string.Format("Time diff should be less than 2 sec, got {0}",(time-point.Time).TotalSeconds));
        }
    }
}
