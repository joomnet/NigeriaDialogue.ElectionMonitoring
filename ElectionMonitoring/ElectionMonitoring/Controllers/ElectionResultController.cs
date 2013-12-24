using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectionMonitoring.Controllers
{
    using ElectionMonitoring.ViewModels;
    using ElectionMonitoring.Business;
    using ElectionMonitoring.Models;
    using AutoMapper;
    
    public class ElectionResultController : Controller
    {
        IRaceResultService service = new RaceResultService();
        
        //
        // GET: /ElectionResult/
        [HttpGet]
        public ActionResult RaceResults()
        {
            
           return  View(new RaceResultsViewModel
            {
                Title = "Presidential election results"
            });
        }
        
        [HttpPost]
        public ActionResult RaceResults(RaceResultsViewModel viewModel)
        {
            try
            {
                var regionalResults = new List<RegionResultViewModel>();
                var overallResults = new RegionResultViewModel();

                //get all results for the given race
                var allRegionsResults = service.GetAggregatedRaceResults(viewModel.RaceID, null).ToList();
                var regions = service.GetRegions().Where(r => r.TopLevel == true).ToList(); //get all toplevel regions
                Mapper.CreateMap<Models.AggregatedRaceResult, ViewModels.AggregatedRaceResultViewModel>();
                //get result per region
                foreach (var region in regions)
                {
                    var regionresult = new RegionResultViewModel();
                    regionresult.Region = new RegionViewModel { RegionID = region.RegionID, RegionCode = region.RegionCode, Name = region.Name };
                    regionresult.Results = Mapper.Map<List<Models.AggregatedRaceResult>, List<ViewModels.AggregatedRaceResultViewModel>>
                        (allRegionsResults.Where(rr => rr.RegionCode.ToLower() == region.RegionCode.ToLower()).ToList());
                    regionresult.Winner = regionresult.Results.OrderByDescending(rr => rr.TotalVotes).FirstOrDefault();
                    regionalResults.Add(regionresult);
                }

                // overall results
                // aggregate for all regions
                var aggResults = allRegionsResults.GroupBy(r => r.PartyAcronym)
                                    .Select(r =>
                                        new
                                        {
                                            PartyAcronym = r.Key,
                                            RegionID = r.FirstOrDefault().RegionID,
                                            RegionCode = r.FirstOrDefault().RegionCode,
                                            RegionName = r.FirstOrDefault().RegionName,
                                            RaceID = r.FirstOrDefault().RaceID,
                                            CandidateID = r.FirstOrDefault().CandidateID,
                                            FirstName = r.FirstOrDefault().FirstName,
                                            LastName = r.FirstOrDefault().LastName,
                                            MiddleName = r.FirstOrDefault().MiddleName,
                                            PartyName = r.FirstOrDefault().PartyName,
                                            PartyColor = r.FirstOrDefault().PartyColor,
                                            TotalVotes = r.Sum(p => p.TotalVotes),
                                        }).ToList();

                overallResults.Results = new List<AggregatedRaceResultViewModel>();
                foreach (var aggResult in aggResults)
                {
                    overallResults.Results.Add(new AggregatedRaceResultViewModel
                    {
                        PartyAcronym = aggResult.PartyAcronym,
                        RegionID = aggResult.RegionID,
                        RegionCode = aggResult.RegionCode,
                        RegionName = aggResult.RegionName,
                        RaceID = aggResult.RaceID,
                        CandidateID = aggResult.CandidateID,
                        TotalVotes = aggResult.TotalVotes,
                        FirstName = aggResult.FirstName,
                        LastName = aggResult.LastName,
                        MiddleName = aggResult.MiddleName,
                        PartyName = aggResult.PartyName,
                        PartyColor = aggResult.PartyColor,
                    });
                }

                var allResults = new RaceResultsViewModel
                {
                    RegionalResults = regionalResults,
                    SelectedRegionResults = string.IsNullOrEmpty(viewModel.RegionCode)
                                            ? overallResults
                                            : regionalResults.Where(rr => rr.Region.RegionCode == viewModel.RegionCode).FirstOrDefault()
                };


                return Json(allResults, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, StackTrace = ex.StackTrace, InnerException = ex.InnerException.Message });
            }
        }

        [HttpPost]
        [Obsolete ("No Longer in use")]
        public ActionResult ElectionMonitoringRaceResults(string regioncode)
        {
            var results = new List<Models.AggregatedRaceResult>();
            var title = "National results of Presidential Elections";
            //TODO: Rework
            //This reall y should try to get for a particualr region 
            //if region is unknown it should return for whoel country
            if ((string.IsNullOrEmpty(regioncode)) || regioncode =="NGA")
            {
                //get 
                results = service.GetAggregatedRaceResults(1 , "").ToList();
                title = "National results of Presidential Elections";
            }
            else
            {
                results = service.GetAggregatedRaceResults(1,regioncode).ToList();
                
                title = regioncode +" state results of Presidential Elections";
            }

            if (results.Count < 1)
                title = "No results found";
            
            string[][] data = new string[results.Count][];


            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                var name = result.PartyAcronym + " ("
                    + result.FirstName.Substring(0, 1) + "."
                    + result.LastName + ")";
                name = result.PartyAcronym;
                data[i] = new string[] { name, result.TotalVotes.ToString() };
            }

            var returnValue = new RaceResultsViewModel
            {
                Title = title,
                PieData = data
            };

            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EnterResults()
        {
         //   return Json(null, JsonRequestBehavior.AllowGet);
            return View(new EnterResultsViewModel());
        }

        [HttpPost]
        public ActionResult EnterResults(RaceResultsViewModel viewModel)
        {
            bool created = false;
            viewModel.SubmittedOn = DateTime.Now;
            Mapper.CreateMap<ViewModels.RaceResultsViewModel, Models.RaceResult>();
            var raceResult = Mapper.Map<ViewModels.RaceResultsViewModel, Models.RaceResult>(viewModel);
            raceResult.ApprovedBy = null;
            raceResult.ApprovedOn = null;
            raceResult.ModifiedBy = null;
            raceResult.ModifiedOn = null;
            var raceResultID = service.CreateRaceResult(raceResult);
            if (raceResultID > 0)
                created = true;
            return Json(new { Created = created }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLists()
        {
            try
            {
                var regions = service.GetRegions().ToList();
                regions = regions.Where(r => r.TopLevel == true).ToList();
                var raceTypes = service.GetRaceTypes().ToList();
                var raceid = 1; //for now
                var candidates = service.GetCandidates().Where(c => c.RaceID == raceid).ToList();
                var allparties = service.GetParties();
                var parties = new List<Party>();
                foreach (var candidate in candidates)
                {
                    parties.Add(allparties.Where(p => p.PartyID == candidate.PartyID).FirstOrDefault());
                }
                return Json(new { Regions = regions, RaceTypes = raceTypes, Candidates = candidates, Parties = parties }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, StackTrace = ex.StackTrace, InnerException = ex.InnerException.Message });
            }
        }
        
        [HttpPost]
        public ActionResult GetSubRegions(string regioncode)
        {
            var regions = new List<Region>();
            if (!string.IsNullOrEmpty (regioncode ))
            {
                //get region with
                var selectstate = service.GetRegions().Where(r => r.RegionCode.ToLower() == regioncode.ToLower()).FirstOrDefault();
                regions = service.GetRegions().ToList();
                regions = regions.Where(r => r.ParentRegionID == selectstate.RegionID).ToList();
            }
            
            return Json(new { SubRegions = regions}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCandidates(int raceid)
        {
            //get region with 
            var candidates = service.GetCandidates().Where(c =>c.RaceID == raceid ).ToList ();            
            return Json(new { Candidates = candidates}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetParty(int partyid)
        {
            var party = service.GetParties().Where(p => p.PartyID == partyid).FirstOrDefault();
            return Json(party, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Donation()
        {
            return View(new ViewModels.DonationViewModel());
        }

    }
}
