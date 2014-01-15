using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionMonitoring.DTO
{
    public class AggregatedDonation
    {
        public int ProjectID { get; set; }
        public int DonorID { get; set; }
        public string DonorName { get; set; }
        public decimal SumOfDonations { get; set; }
    }
}