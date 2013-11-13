using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Models
{
    public class RaceResult
    {
        public int RaceResultID { get; set; }
        public int RaceID { get; set; }
        public int CandidateID { get; set; }
        public int RegionID { get; set; }
        public int NoOfVotes { get; set; }
        public int SubmittedBy { get; set; }
        public DateTime SubmittedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
    }
}
