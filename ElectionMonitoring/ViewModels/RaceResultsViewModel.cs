using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using ElectionMonitoring.Business;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.ViewModels
{
    public class RaceResultsViewModel
    {
        public List<Models.Region> Regions {
            get
            {
                return new RaceResultService().GetRegions().Where(r => r.TopLevel == true).ToList();
            }
        }

        public int RaceID { get; set; }
        public string RegionCode { get; set; }
        public string Color { get; set; }
        public List<AggregatedRaceResult> CummulativeResults { get; set; }
        public List<AggregatedRaceResult> RegionalResults { get; set; }

        

        /// <summary>
        ///  NOT sure the properties below this line are needded
        /// </summary>
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

    //public class ChartData : IEnumerable
    //{
    //    public int Rows { get; set; }
    //    public int Columns { get; set; }
    //    public IEnumerator GetEnumerator()
    //    {
    //        for (int i = 0; i < Rows; i++)
    //        {
    //            string[] result = new string[Columns];
    //            for (int j = 0; j < Columns; j++)
    //            {
    //                result[j] = this[i, j];
    //            }
    //            yield return result;
    //        }
    //    }

    //    public string[][] ToJaggedArray()
    //    {
    //        var jaggedArray = new string[Rows][];

    //        for (int i = 0; i < Rows; ++i)
    //        {
    //            jaggedArray[i] = new string[Columns];

    //            for (int j = 0; j < Columns; j++)
    //            {
    //                jaggedArray[i][j] = this[i, j];
    //            }
    //        }

    //        return jaggedArray;
    //    }

    //}
}