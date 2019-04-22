using System;
using System.Collections.Generic;

namespace TailWaters.Data.Models
{
    public partial class Subscriber
    {
        public int SubscriberId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Number { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
