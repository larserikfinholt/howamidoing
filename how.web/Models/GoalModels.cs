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
        Dayly,
        Weekly,
        Monthly
    }

    public enum UnitType
    {
        [Description("Goal is measured in number of times")]
        Number,
        [Description("Goal is measured in minutes of an activity")]
        Minutes,
        [Description("Goal is measured in hours of an activity")]
        Hours,
    }

    public class Goal
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        [Required]
        public string Title { get; set; }
        public int Interval { get; set; }
        public IntervalType IntervalType { get; set; }
        public UnitType Unit { get; set; }
        public List<DoneIt> DoneIts { get; set; }
    }
    public class DoneIt
    {
        [Key]
        public int Id { get; set; }
        public int GoalId { get; set; }
        public Goal Goal { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public string Comment { get; set; }
    }
}