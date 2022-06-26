using System;

namespace Backend
{
    public class Task
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int TimeInMinutes { get; set; }
        public DateTime DataDeadline { get; set; }
        public int Importance { get; set; }
        public DateTime Beginning { get; set; }
        public DateTime Ending { get; set; }
        public bool IsEnoughTime { get; set; }
        public bool IsFixed { get; set; }
        public TaskStatus Status { get; set; }

        public Task() { }

        public Task(string name,
            int timeInMinutes = 0,
            DateTime? dataDeadline = null,
            int importance = -1,
            bool isEnoughTime = true,
            bool isFixed = false,
            TaskStatus status = TaskStatus.Relevant)
        {
            Name = name;
            TimeInMinutes = timeInMinutes;
            DataDeadline = dataDeadline ?? DateTime.MinValue;
            Importance = importance;
            Beginning = dataDeadline.Value.AddMinutes(-timeInMinutes);
            Ending = dataDeadline.Value;
            IsEnoughTime = isEnoughTime;
            IsFixed = isFixed;
            Status = status;
        }
    }
}