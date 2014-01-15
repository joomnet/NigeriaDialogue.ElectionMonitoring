namespace ElectionMonitoring.ViewModels
{
    public class AggregatedRaceResultViewModel
    {
        public int? RaceID { get; set; }
        public int? CandidateID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + MiddleName.Substring(0, 1) + ". " + LastName; } }
        public string PartyName { get; set; }
        public string PartyAcronym { get; set; }
        public int? RegionID { get; set; }
        public string RegionName { get; set; }
        public string RegionCode { get; set; }
        public int TotalVotes { get; set; }
        public string PartyColor { get; set; }
    }
}