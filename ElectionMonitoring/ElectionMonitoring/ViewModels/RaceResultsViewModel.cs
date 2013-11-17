using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using ElectionMonitoring.Business;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.ViewModels
{
    public class RegionViewModel
    {
        public int RegionID { get; set; }
        //public bool TopLevel { get; set; }
        //public int ParentRegionID { get; set; }
        //public int StatusID { get; set; }
        public string Name { get; set; }
        //public int RegionTypeID { get; set; }
        public string RegionCode { get; set; }
        public string Coordinates { get; set; }
    }
    public class AggregatedRaceResultViewModel
    {
        public int RaceID { get; set; }
        public int CandidateID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + MiddleName.Substring(0, 1) + ". " + LastName; } }
        public string PartyName { get; set; }
        public string PartyAcronym { get; set; }
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public string RegionCode { get; set; }
        public int TotalVotes { get; set; }
        public string PartyColor { get; set; }
    }

    public class RegionResultViewModel
    {
        public RegionViewModel Region { get; set; }
        public List<AggregatedRaceResultViewModel> Results { get; set; }
        public AggregatedRaceResultViewModel Winner { get; set; }
    }

    public class RaceResultsViewModel
    {
        public List<Models.Region> Regions {
            get
            {
                return new RaceResultService().GetRegions().Where(r => r.TopLevel == true).ToList();
            }
        }

        public List<RegionResultViewModel> RegionalResults { get; set; }
        public RegionResultViewModel SelectedRegionResults { get; set; }



        public int RaceID { get; set; }
        public string RegionCode { get; set; }
        //public string Color { get; set; }
        //public List<AggregatedRaceResult> AllResults { get; set; }
        //public List<AggregatedRaceResult> SelectedRegionResultss { get; set; }
                

        public int CandidateID { get; set; }
        public int RegionID { get; set; }
        public int NoOfVotes { get; set; }
        public int SubmittedBy { get; set; }
        public DateTime SubmittedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }

        public string Title { get; set; }
        public string[] LineData { get; set; }
        public string [][] PieData { get; set; }
        public string[][] ChartData { get; set; }

    }
}