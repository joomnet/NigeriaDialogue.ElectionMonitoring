using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Models
{
    public class Donor
    {
        public int DonorID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }
    }
}
