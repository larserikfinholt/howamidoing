using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using how.web;
using how.web.Business;
using how.web.Models;
using how.web.ViewModel;
using System.Collections.Generic;

namespace how.test
{
    [TestClass]
    public class ProcessorTests
    {
        [TestMethod]
        public void when_no_doneits_it_should_show_status_not_started()
        {
            var target = new GoalProcessor();
            var goal = new Goal() { Amount = 7, IntervalType = IntervalType.Weekly };
            var now = DateTime.Now;

            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.NotStarted, vm.Status);
            //goal.DoneIts.Add(new DoneIt() { Amount = 1, Date = now.a

        }
        [TestMethod]
        public void when_first_doneit_it_should_start_counting_down()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { Amount = 7, IntervalType = IntervalType.Weekly };

            goal.DoneIts.Add(new DoneIt() { Amount = 1, Date = now });

                
            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);
        }
        [TestMethod]
        public void it_should_calculate_correct_hours_left_to_zero()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { Amount = 7, IntervalType = IntervalType.Weekly };

            // Week
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 7.01M, Date = now.AddDays(-7) } };
            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);

            goal.DoneIts = new List<DoneIt>(){ new DoneIt() { Amount = 6.99M, Date = now.AddDays(-7) }};
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);

            // Hour
            goal = new Goal() { Amount = 24, IntervalType = IntervalType.Dayly };
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 24.01M, Date = now.AddDays(-1) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);

            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 23.99M, Date = now.AddDays(-1) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);

            // Month
            goal = new Goal() { Amount = 3, IntervalType = IntervalType.Monthly };
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 3.01M, Date = now.AddDays(-30) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);

            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 2.99M, Date = now.AddDays(-30) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);
        }

        [TestMethod]
        public void it_should_increase_hours_left_when_adding_a_new_doneit()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);

            // Week
            var goal = new Goal() { Amount = 7, IntervalType = IntervalType.Weekly };
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 3.01M, Date = now.AddDays(-7) } };
            goal.DoneIts.Add(new DoneIt() { Amount = 3, Date = now.AddDays(-4) } );
            goal.DoneIts.Add(new DoneIt() { Amount = 1, Date = now.AddDays(-1) });
            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);

            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 2.9M, Date = now.AddDays(-7) } };
            goal.DoneIts.Add(new DoneIt() { Amount = 3, Date = now.AddDays(-4) });
            goal.DoneIts.Add(new DoneIt() { Amount = 1, Date = now.AddDays(-1) });
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);
        }
        [TestMethod]
        public void overallstatus_should_be_showing_worst_case()
        {
            var target = new GoalProcessor();
            Assert.AreEqual(GoalStatus.NoGoalsDefined, target.FindOverallStatus(new List<GoalViewModel>()));
            Assert.AreEqual(GoalStatus.OnTrack, target.FindOverallStatus(new List<GoalViewModel>() { new GoalViewModel { Status=GoalStatus.NotStarted }, new GoalViewModel { Status=GoalStatus.OnTrack} }));
            Assert.AreEqual(GoalStatus.Behind, target.FindOverallStatus(new List<GoalViewModel>() { new GoalViewModel { Status = GoalStatus.Behind }, new GoalViewModel { Status = GoalStatus.OnTrack } }));
        }

        [TestMethod]
        public void it_should_never_go_below_a_configurable_threshold()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { Amount = 7, IntervalType = IntervalType.Weekly };

            // Hour
            goal = new Goal() { Amount = 24, IntervalType = IntervalType.Dayly };
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 24.01M, Date = now.AddDays(-1) } };
            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);
            Assert.IsTrue(Math.Abs(vm.CurrentLevel - 0) < 0.1M);

            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 23.99M, Date = now.AddDays(-1) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);
            Assert.IsTrue(Math.Abs(vm.CurrentLevel - 0) < 0.1M);

            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 24.01M, Date = now.AddDays(-2) } };
            vm = target.ProcessGoal(goal);
            Assert.IsTrue(Math.Abs(vm.CurrentLevel - -24) < 0.1M);

            // Hour limited by threshold (2*)
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 24.01M, Date = now.AddDays(-4) } };
            vm = target.ProcessGoal(goal);
            Assert.IsTrue(Math.Abs(vm.CurrentLevel - -48) < 0.1M);

        }
        [TestMethod]
        public void it_should_build_a_flat_zero_line_if_no_donits()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { IntervalType = IntervalType.Weekly };

            var vm= target.ProcessGoal(goal);
            Assert.AreEqual(vm.Graph.Points.Count, 2);
        }
       

    }
}
