using System;
using System.Collections.Generic;

namespace TailWaters.Data.Models
{
    public partial class TailWater
    {
        public TailWater()
        {
            Schedules = new HashSet<Schedule>();
        }

        public int TailWaterId { get; set; }
        public int OperatorId { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }

        public virtual Operator Operator { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
