using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Models
{
    public class AggregatedRaceResult
    {
        public int RaceID { get; set; }
        public int CandidateID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PartyName { get; set; }
        public string PartyAcronym { get; set; }
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public string RegionCodme { get; set; }
        public int TotalVotes { get; set; }
    }
}
