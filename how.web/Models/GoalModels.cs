using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace how.web.Models
{
    public enum IntervalType
    {
        [Description("Goal is evaluted dayly")]
        Dayly=0,
        Weekly=1,
        Monthly=2
    }

    public enum GoalEvaluation
    {
        [Description("Goal is evaluated continuously")]
        Countinious=0,
        [Description("Goal would be evaluated on the end date")]
        Timelimited=1
    }

    public enum GoalMeasurement
    {
        [Description("in amounts (time, number etc)")]
        Amount,
        [Description("by checkpoints")]
        Checklist
    }

    

    public class Goal
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        [Description("Is this goal currently enabled")]
        public bool Enabled { get; set; }
        [Required]
        [Description("Enter your goal here. (e.g 'Go biking 4 hours a week'")]
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string ChecklistItems { get; set; }
        public int IntervalTypeValue { get; set; }
        public int EvaluationTypeValue { get; set; }
        public int MeasurementTypeValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Unit { get; set; }
        [Description("How do you measure goal progress?")]
        public GoalMeasurement MesurementType
        {
            get { return (GoalMeasurement)MeasurementTypeValue; }
            set { MeasurementTypeValue = (int)value; }
        }
        public IntervalType IntervalType 
        {
            get { return (IntervalType)IntervalTypeValue; }
            set { IntervalTypeValue = (int)value; }
        }
        [Description("When will your goal be evaluated")]
        public GoalEvaluation EvaluationType
        {
            get { return (GoalEvaluation)EvaluationTypeValue; }
            set { EvaluationTypeValue = (int)value; }
        }
        public List<DoneIt> DoneIts { get; set; }
        public Goal()
        {
            DoneIts = new List<DoneIt>(); 
        }
    }
    public class DoneIt
    {
        [Key]
        public int Id { get; set; }
        public int GoalId { get; set; }
        public Goal Goal { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }
}