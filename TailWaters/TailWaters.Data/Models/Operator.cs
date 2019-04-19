using System;
using System.Collections.Generic;

namespace TailWaters.Data.Models
{
    public partial class Operator
    {
        public Operator()
        {
            TailWaters = new HashSet<TailWater>();
        }

        public int OperatorId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TailWater> TailWaters { get; set; }
    }
}
