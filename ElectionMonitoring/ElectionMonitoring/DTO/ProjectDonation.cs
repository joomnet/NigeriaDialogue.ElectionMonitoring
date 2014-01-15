using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionMonitoring.DTO
{
    public class ProjectDonation
    {
        public List<DTO.AggregatedDonation> Donations { get; set; }
        public decimal TotalDonations
        {
            get
            {
                return Donations.Sum(b => b.SumOfDonations);
            }
        }
    }
}