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
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 48.01M, Date = now.AddDays(-2) } };
            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.OnTrack, vm.Status);

            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 47.99M, Date = now.AddDays(-2) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);

            // Hour limited by threshold (2*)
            goal = new Goal() { Amount = 24, IntervalType = IntervalType.Dayly };
            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 48.01M, Date = now.AddDays(-3) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);

            goal.DoneIts = new List<DoneIt>() { new DoneIt() { Amount = 47.99M, Date = now.AddDays(-3) } };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(GoalStatus.Behind, vm.Status);
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
        [TestMethod]
        public void it_should_start_one_interval_before_and_end_one_interval_after_if_interval()
        {
            var now = DateTime.Now;
            var target = new GoalProcessor(now);
            var goal = new Goal() {EvaluationType=GoalEvaluation.Countinious, IntervalType = IntervalType.Weekly };

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
            var goal = new Goal() {  EvaluationType=GoalEvaluation.Timelimited };
            
            var vm = target.ProcessGoal(goal);
            Assert.AreEqual(0, vm.Graph.Points[0].Amount);
            Assert.AreEqual(0, vm.Graph.Points[1].Amount);

            goal = new Goal() { EvaluationType = GoalEvaluation.Timelimited, StartDate=now.AddDays(-10), EndDate=now.AddDays(10) };
            vm = target.ProcessGoal(goal);
            Assert.AreEqual(vm.Graph.Points[0].Time, now.AddDays(-10));
            Assert.AreEqual(vm.Graph.Points[1].Time, now.AddDays(10));


            
        }

    }
}
