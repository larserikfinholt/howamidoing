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
        public void it_should_show_a_stright_line_when_in_cutoff()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { Amount = 7, IntervalType = IntervalType.Weekly };

            
            // Simple one entry
            // -28,7
            // -21,0
            // -14,-7
            // -7, -14
            // -6, -14 (cutoff)
            // 0, -14 (cutoff)
            goal.DoneIts = new List<DoneIt>() { 
                new DoneIt() { Amount = 7, Date = now.AddDays(-28) } ,
                new DoneIt() { Amount = 0, Date = now.AddDays(-21) } ,
                new DoneIt() { Amount = 0, Date = now.AddDays(-14) } ,
                new DoneIt() { Amount = 0, Date = now.AddDays(-7) } ,
            };
            var vm = target.ProcessGoal(goal);
            // 1: 7
            AssertPointOnGraph(vm, 0, 0, now.AddDays(-28));
            AssertPointOnGraph(vm, 1, 7, now.AddDays(-28));
            // 2: 0
            AssertPointOnGraph(vm, 2, 0, now.AddDays(-21));
            AssertPointOnGraph(vm, 3, 0, now.AddDays(-21));
            // 3: 0
            AssertPointOnGraph(vm, 4, -7, now.AddDays(-14));
            AssertPointOnGraph(vm, 5, -7, now.AddDays(-14));
            // 4: 0
            AssertPointOnGraph(vm, 6, -14, now.AddDays(-7));
            AssertPointOnGraph(vm, 7, -14, now.AddDays(-7));
            // Cutoff
            AssertPointOnGraph(vm, 8, -14, now.AddDays(-7));
            // Cutoff
            AssertPointOnGraph(vm, 9, -14, now.AddDays(-7));
            // Final
            AssertPointOnGraph(vm, 10, -14, now.AddDays(0));

        }
        [TestMethod]
        public void it_should_show_a_stright_line_when_in_cutoff2()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { Amount = 1, IntervalType = IntervalType.Dayly };


            // Simple one entry
            // -8,1
            goal.DoneIts = new List<DoneIt>() { 
                new DoneIt() { Amount = 1, Date = now.AddDays(-10) } ,
            };
            var vm = target.ProcessGoal(goal);
            // Start 0,1
            AssertPointOnGraph(vm, 0, 0, now.AddDays(-10));
            AssertPointOnGraph(vm, 1, 1, now.AddDays(-10));
            // 
            AssertPointOnGraph(vm, 2, -2, now.AddDays(-8));
            AssertPointOnGraph(vm, 3, -2, now);
        }

        [TestMethod]
        public void it_should_calcualte_cutofftime_correct()
        {
            var now = DateTime.Now;
            var firstDoneit = new DateTime(2012, 5, 27);
            var target = new GoalProcessor(now);
            var goal = new Goal() { Amount = 1, IntervalType = IntervalType.Dayly };

            var calculated = target.CalculateLowerCutoffTime(goal, 1, firstDoneit);
            AssertTimesAreAlmostEqual(firstDoneit.AddDays(3), calculated);

            calculated = target.CalculateLowerCutoffTime(goal, 0, now);
            AssertTimesAreAlmostEqual(now.AddDays(2), calculated);

            calculated = target.CalculateLowerCutoffTime(goal, 1M, now.AddDays(-10));
            AssertTimesAreAlmostEqual(now.AddDays(-7), calculated);
        }

        private void AssertTimesAreAlmostEqual(DateTime now, DateTime calculated)
        {
            Assert.IsTrue(Math.Abs((now - calculated).TotalSeconds) < 2, string.Format("Time diff should be less than 2 sec, got {0}", (now - calculated).TotalSeconds));
        }

        private static void AssertPointOnGraph(web.ViewModel.GoalViewModel vm, int pointNo, decimal value, DateTime time)
        {
            var point = vm.Graph.Points.OrderBy(x => x.Time).ToList()[pointNo];
            Assert.IsTrue(Math.Abs(value - point.y) < (decimal)0.0001, string.Format("Diff should be zero, got {0}", value-point.y));
            Assert.IsTrue(Math.Abs((time-point.Time).TotalSeconds)<2, string.Format("Time diff should be less than 2 sec, got {0}",(time-point.Time).TotalSeconds));
        }
    }
}
