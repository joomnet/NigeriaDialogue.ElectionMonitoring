using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionMonitoring.DTO
{
    public class Candidate
    {
        public int CandidateID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int PartyID { get; set; }
        public Nullable<int> RaceID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string Gender { get; set; }
    }
}