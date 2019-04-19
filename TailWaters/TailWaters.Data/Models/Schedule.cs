using System;
using System.Collections.Generic;

namespace TailWaters.Data.Models
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int TailWaterId { get; set; }
        public int Hour { get; set; }
        public double ProjectedFlow { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual TailWater TailWater { get; set; }
    }
}
