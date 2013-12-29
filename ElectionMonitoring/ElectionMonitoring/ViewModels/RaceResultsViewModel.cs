using System;
using System.Collections.Generic;
using System.Linq;
using ElectionMonitoring.Business;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.ViewModels
{
    public class RaceResultsViewModel
    {
        public List<Region> Regions {
            get
            {
                return new RaceResultService().GetRegions().Where(r => r.TopLevel.Value).ToList();
            }
        }

        public List<RegionResultViewModel> RegionalResults { get; set; }
        public RegionResultViewModel SelectedRegionResults { get; set; }
        public int RaceID { get; set; }
        public string RegionCode { get; set; }
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