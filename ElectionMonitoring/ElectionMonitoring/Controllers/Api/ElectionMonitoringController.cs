using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using ElectionMonitoring.Business;
using ElectionMonitoring.Filters;
using ElectionMonitoring.Models;
using ElectionMonitoring.Repository;
using Microsoft.Practices.Unity.Utility;

namespace ElectionMonitoring.Controllers.Api
{
    [ElectionMonitoringExceptionFilter]
    public class ElectionMonitoringController : ApiController
    {
        private static IRaceResultService _raceResultRepository; // = new RaceResultService();
        private static IRegionRepository _regionRepository; // = new RegionRepository();
        private static IRaceRepository _raceRepository; // = new RaceRepository();

        public ElectionMonitoringController()
        {
        }

        public ElectionMonitoringController(IRaceResultService raceResultRepository,
                                            IRegionRepository regionRepository, IRaceRepository raceRepository)
        {
            Guard.ArgumentNotNull(raceRepository, "raceRepository");
            Guard.ArgumentNotNull(regionRepository, "regionRepository");
            Guard.ArgumentNotNull(raceResultRepository, "raceResultRepository");

            _raceResultRepository = raceResultRepository;
            _regionRepository = regionRepository;
            _raceRepository = raceRepository;
        }

        #region Misc

        [HttpGet]
        public IEnumerable<string> About()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
                                                                      TimeSpan.TicksPerDay*version.Build +
                                                                      // days since 1 January 2000
                                                                      TimeSpan.TicksPerSecond*2*version.Revision));
                // seconds since midnight, (multiply by 2 to get original)

            var returnValue = new List<string>();
            returnValue.Add("Service : Election Monitoring Service");
            returnValue.Add("Version : " + version);
            returnValue.Add("Build Date : " + buildDateTime.ToLongDateString() + " " + buildDateTime.ToLongTimeString());
            returnValue.Add("Data LastUpdated : 19th November 2013 10:21 GMT");
            returnValue.Add("Author : Sola Oderinde");
            returnValue.Add("Organisation : Nigeria Dialogue");

            return returnValue;
        }

        #endregion

        #region Races

        // GET api/electionresults/races (Get all election races)
        [HttpGet]
        [ActionName("Races")]
        public IEnumerable<Race> GetRaces()
        {
            var races = new List<Race>();
            races = (List<Race>) _raceRepository.GetRaces();
            return races;
        }

        // GET api/electionresults/races/1 (Get race with given id)
        [HttpGet]
        [ActionName("Races")]
        public Race GetRace(int id)
        {
            return _raceRepository.GetRace(id);
        }

        #endregion

        #region Regions

        // GET api/electionresults/regions (Get all regions)
        [HttpGet]
        [ActionName("Regions")]
        public IEnumerable<Region> GetRegions()
        {
            List<Region> regions = _regionRepository.GetRegions().Where(r => r.TopLevel == true).ToList();
            return regions;
        }

        // GET api/electionresults/regions/AB  or api/electionresults/regions/1 (Get region with region code or id)
        [HttpGet]
        [ActionName("Regions")]
        public Region GetRegions(string regioncode)
        {
            int regionid = 0;
            bool parsed = Int32.TryParse(regioncode, out regionid);
            var region = new Region();
            region = parsed
                         ? _regionRepository.GetRegions().FirstOrDefault(r => r.RegionID == regionid)
                         : _regionRepository.GetRegions()
                                            .FirstOrDefault(r => r.RegionCode.ToLower() == regioncode.ToLower());

            return region;
        }


        // GET api/electionresults/subregions (Get all)
        [HttpGet]
        [ActionName("SubRegions")]
        public IEnumerable<Region> GetSubRegions(int id)
        {
            var regions = new List<Region>();
            // if no regioncode was given get top level regions
            if (id <= 0)
            {
                regions = _regionRepository.GetRegions().Where(r => r.TopLevel == true).ToList();
            }
            else
            {
                IEnumerable<Region> allregions = _regionRepository.GetRegions();
                regions = allregions.Where(r => r.ParentRegionID == id).ToList();
            }
            return regions;
        }

        // GET api/electionresults/regions (Get regions with regioncode)
        [HttpGet]
        [ActionName("SubRegions")]
        public IEnumerable<Region> GetSubRegions(string regioncode)
        {
            var regions = new List<Region>();
            // if no regioncode was given get top level regions
            if (string.IsNullOrEmpty(regioncode))
            {
                regions = _regionRepository.GetRegions().Where(r => r.TopLevel == true).ToList();
            }
            else
            {
                IEnumerable<Region> allregions = _regionRepository.GetRegions();
                Region selectedRegion = allregions.FirstOrDefault(r => r.RegionCode.ToLower() == regioncode.ToLower());
                regions = allregions.Where(r => r.ParentRegionID == selectedRegion.RegionID).ToList();
            }
            return regions;
        }

        #endregion

        #region Results

        // GET api/electionresults/raceresults/1 (Get results by raceid [results aggregated in subregions)    
        [HttpGet]
        [ActionName("RaceResults")]
        public IEnumerable<AggregatedRaceResult> GetRaceResults(int id)
        {
            var result = (List<AggregatedRaceResult>) _raceResultRepository.GetAggregatedRaceResults(id, null);

            return result;
        }

        // GET api/electionresults/1  (Get results by raceid and regioncode if given)    
        [HttpGet]
        [ActionName("RaceResults")]
        public IEnumerable<AggregatedRaceResult> GetRaceResults(int id, string regioncode)
        {
            var result = new List<AggregatedRaceResult>();
            try
            {
                if (String.IsNullOrEmpty(regioncode) ||
                    (regioncode.ToUpper() == "ALL") ||
                    (regioncode.ToUpper() == "NGA"))
                {
                    result = (List<AggregatedRaceResult>) _raceResultRepository.GetAggregatedRaceResults(id, null);
                    result = result.GroupBy(r => r.PartyAcronym)
                                   .Select(r =>
                                       {
                                           AggregatedRaceResult aggregatedRaceResult = r.FirstOrDefault();
                                           return new AggregatedRaceResult
                                               {
                                                   PartyAcronym = r.Key,
                                                   RegionID = aggregatedRaceResult.RegionID,
                                                   RegionCode = "ALL",
                                                   //RegionName = r.FirstOrDefault().RegionName,
                                                   RaceID = aggregatedRaceResult.RaceID,
                                                   CandidateId = aggregatedRaceResult.CandidateId,
                                                   FirstName = aggregatedRaceResult.FirstName,
                                                   LastName = aggregatedRaceResult.LastName,
                                                   MiddleName = aggregatedRaceResult.MiddleName,
                                                   PartyName = aggregatedRaceResult.PartyName,
                                                   PartyColor = aggregatedRaceResult.PartyColor,
                                                   TotalVotes = r.Sum(p => p.TotalVotes),
                                               };
                                       }).ToList();
                }
                else
                {
                    result = (List<AggregatedRaceResult>) _raceResultRepository.GetAggregatedRaceResults(id, regioncode);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        // GET api/electionresults/1  (Get results for a given region in race)

        #endregion
    }
}