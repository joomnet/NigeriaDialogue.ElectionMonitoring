﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Models
{
    public class Donation
    {
        public int DonationID { get; set; }
        public int DonorID { get; set; }
        //public Donor Donor { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonationDate { get; set; }
        public int ProjectID { get; set; }
    }
}
