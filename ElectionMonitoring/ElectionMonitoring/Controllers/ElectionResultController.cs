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
        public ActionResult RaceResults(string regioncode)
        {
            var results = new List<Models.AggregatedRaceResult>();
            var title = "National results of Presidential Elections";
            //TODO: Rework
            //This reall y should try to get for a particualr region 
            //if region is unknown it should return for whoel country
            if ((string.IsNullOrEmpty(regioncode)) || regioncode =="NGA")
            {
                //get 
                results = service.GetAggregatedRaceResults(1).ToList();
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

        [HttpGet]
        public ActionResult GetLists()
        {
            var regions = service.GetRegions().Where(r => r.TopLevel == true).ToList();
            var raceTypes = service.GetRaceTypes().ToList();
            var raceid = 1; //for now
            var candidates = service.GetCandidates().Where(c => c.RaceID == raceid).ToList();
            var allparties = service.GetParties();
            var parties = new List<Party>();
            foreach (var candidate in candidates)
            {
                parties.Add (allparties.Where(p => p.PartyID == candidate.PartyID ).FirstOrDefault ());
            }
            return Json(new { Regions = regions, RaceTypes = raceTypes, Candidates = candidates, Parties = parties }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
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

        [HttpGet]
        public ActionResult GetCandidates(int raceid)
        {
            //get region with 
            var candidates = service.GetCandidates().Where(c =>c.RaceID == raceid ).ToList ();            
            return Json(new { Candidates = candidates}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetParty(int partyid)
        {
            var party = service.GetParties().Where(p => p.PartyID == partyid).FirstOrDefault();
            return Json(party, JsonRequestBehavior.AllowGet);
        }

    }
}
