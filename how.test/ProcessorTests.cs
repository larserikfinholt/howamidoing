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
            var goal = new Goal() { Interval = 7, IntervalType = IntervalType.Weekly, Unit = UnitType.Number };
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
            var goal = new Goal() { Interval = 7, IntervalType = IntervalType.Weekly, Unit = UnitType.Number };

            goal.DoneIts.Add(new DoneIt() { Amount = 1, Date = now });

                
            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);
        }
        [TestMethod]
        public void it_should_calculate_correct_hours_left_to_zero()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() { Interval = 7, IntervalType = IntervalType.Weekly, Unit = UnitType.Number };

            // Week
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 7.01M, Date = now.AddDays(-7) } };
            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);

            goal.DoneIts = new List<DoneIt>(){ new DoneIt() { Amount = 6.99M, Date = now.AddDays(-7) }};
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);

            // Hour
            goal = new Goal() { Interval = 24, IntervalType = IntervalType.Dayly, Unit = UnitType.Number };
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 24.01M, Date = now.AddDays(-1) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);

            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 23.99M, Date = now.AddDays(-1) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);

            // Month
            goal = new Goal() { Interval = 3, IntervalType = IntervalType.Monthly, Unit = UnitType.Number };
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
            var goal = new Goal() { Interval = 7, IntervalType = IntervalType.Weekly, Unit = UnitType.Number };
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

    }
}
