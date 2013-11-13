using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Models
{
    public class Candidate
    {
        public int CandidateID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int PartyID { get; set; }
        public int RaceID { get; set; }
        public int CategoryID { get; set; }
        public string Gender { get; set; }
    }
}
