using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ElectionMonitoring.Business;
using ElectionMonitoring.Filters;
using System.Reflection;

namespace ElectionMonitoring.Controllers.Api
{
    [ElectionMonitoringExceptionFilter]
    public class ElectionMonitoringController : ApiController
    {
        static IRaceResultService _raceResultRepository;// = new RaceResultService();
        static IRegionRepository _regionRepository;// = new RegionRepository();
        static IRaceRepository _raceRepository;// = new RaceRepository();

        public ElectionMonitoringController()
        {
        }

        public ElectionMonitoringController(IRaceResultService raceResultRepository, 
            IRegionRepository regionRepository, IRaceRepository raceRepository)
        {
            if ((raceResultRepository == null) ||
                (regionRepository == null) ||
                (raceRepository == null))
            {
                throw new ArgumentNullException("Could not resolve 1 or more repositories");
            }
            _raceResultRepository = raceResultRepository;
            _regionRepository = regionRepository;
            _raceRepository = raceRepository;
        }

        #region Misc
        [HttpGet]
        public IEnumerable<string> About()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
            TimeSpan.TicksPerDay * version.Build + // days since 1 January 2000
            TimeSpan.TicksPerSecond * 2 * version.Revision)); // seconds since midnight, (multiply by 2 to get original)

            var returnValue = new List<string>();
            returnValue.Add("Service : Election Monitoring Service");
            returnValue.Add("Version : " + version.ToString());
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
        public IEnumerable<Models.Race> GetRaces()
        {
            var races = new List<Models.Race>();
            races = (List<Models.Race>) _raceRepository.GetRaces();
            return races;
        }
        
        // GET api/electionresults/races/1 (Get race with given id)
        [HttpGet]
        [ActionName("Races")]
        public Models.Race GetRace(int id)
        {
            return  _raceRepository.GetRace(id);
        }
        #endregion

        #region Regions
        // GET api/electionresults/regions (Get all regions)
        [HttpGet]
        [ActionName("Regions")]
        public IEnumerable<Models.Region> GetRegions()
        {
            var regions = _regionRepository.GetRegions().Where(r => r.TopLevel == true).ToList();
            return regions;
        }

        // GET api/electionresults/regions/AB  or api/electionresults/regions/1 (Get region with region code or id)
        [HttpGet]
        [ActionName("Regions")]
        public Models.Region GetRegions(string regioncode)
        {
            var regionid = 0;
            var parsed = Int32.TryParse(regioncode, out regionid);
            var region = new Models.Region();
            if (parsed)
                region = _regionRepository.GetRegions()
                    .Where(r => r.RegionID == regionid)
                    .FirstOrDefault();
            else
                region = _regionRepository.GetRegions()
                    .Where(r => r.RegionCode.ToLower() == regioncode.ToLower())
                    .FirstOrDefault();

            return region;
        }
        

        // GET api/electionresults/subregions (Get all)
        [HttpGet]
        [ActionName("SubRegions")]
        public IEnumerable<Models.Region> GetSubRegions(int id)
        {
            var regions = new List<Models.Region>();
            // if no regioncode was given get top level regions
            if (id <= 0)
            {
                regions = _regionRepository.GetRegions().Where(r => r.TopLevel == true).ToList();
            }
            else
            {
                var allregions = _regionRepository.GetRegions();
                regions = allregions.Where(r => r.ParentRegionID == id).ToList();
            }
            return regions;
        }

        // GET api/electionresults/regions (Get regions with regioncode)
        [HttpGet]
        [ActionName("SubRegions")]
        public IEnumerable<Models.Region> GetSubRegions(string regioncode)
        {
            var regions = new List<Models.Region>();
            // if no regioncode was given get top level regions
            if (string.IsNullOrEmpty(regioncode))
            {
                regions = _regionRepository.GetRegions().Where(r => r.TopLevel == true).ToList();
            }
            else
            {
                var allregions = _regionRepository.GetRegions();
                var selectedRegion = allregions.Where(r => r.RegionCode.ToLower() == regioncode.ToLower()).FirstOrDefault();
                regions = allregions.Where(r => r.ParentRegionID == selectedRegion.RegionID).ToList();
            }
            return regions;
        }
        #endregion

        #region Results
        // GET api/electionresults/raceresults/1 (Get results by raceid [results aggregated in subregions)    
        [HttpGet]
        [ActionName("RaceResults")]
        public IEnumerable<Models.AggregatedRaceResult> GetRaceResults(int id)
        {

            var result = (List<Models.AggregatedRaceResult>)_raceResultRepository.GetAggregatedRaceResults(id, null);
           
            return result;
        }

        // GET api/electionresults/1  (Get results by raceid and regioncode if given)    
        [HttpGet]
        [ActionName("RaceResults")]
        public IEnumerable<Models.AggregatedRaceResult> GetRaceResults(int id, string regioncode)
        {
            var result = new List<Models.AggregatedRaceResult>();
            try
            {

                if (String.IsNullOrEmpty(regioncode) ||
                    (regioncode.ToUpper() == "ALL") ||
                    (regioncode.ToUpper() == "NGA"))
                {
                    result = (List<Models.AggregatedRaceResult>)_raceResultRepository.GetAggregatedRaceResults(id, null);
                    result = result.GroupBy(r => r.PartyAcronym)
                                        .Select(r =>
                                            new Models.AggregatedRaceResult
                                            {
                                                PartyAcronym = r.Key,
                                                RegionID = r.FirstOrDefault().RegionID,
                                                RegionCode = "ALL",
                                                //RegionName = r.FirstOrDefault().RegionName,
                                                RaceID = r.FirstOrDefault().RaceID,
                                                CandidateID = r.FirstOrDefault().CandidateID,
                                                FirstName = r.FirstOrDefault().FirstName,
                                                LastName = r.FirstOrDefault().LastName,
                                                MiddleName = r.FirstOrDefault().MiddleName,
                                                PartyName = r.FirstOrDefault().PartyName,
                                                PartyColor = r.FirstOrDefault().PartyColor,
                                                TotalVotes = r.Sum(p => p.TotalVotes),
                                            }).ToList();
                }
                else
                {
                    result = (List<Models.AggregatedRaceResult>)_raceResultRepository.GetAggregatedRaceResults(id, regioncode);
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
