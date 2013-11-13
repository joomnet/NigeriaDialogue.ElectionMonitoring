using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    using ElectionMonitoring;
    using AutoMapper;

    public class RaceResultService : IRaceResultService
    {
        private Data.ElectionMonitoringEntities entities = new Data.ElectionMonitoringEntities();
        public RaceResultService()
        {
        }

        public IEnumerable<Models.RaceResult> GetRaceResults()
        {
            var dataRaceResults = entities.RaceResults;
            Mapper.CreateMap<Data.RaceResult, Models.RaceResult>();
            var modelRaceResults = Mapper.Map<IEnumerable<Data.RaceResult>, IEnumerable<Models.RaceResult>>(dataRaceResults);
            return modelRaceResults;
        }

        public IEnumerable<Models.RaceResult> GetRaceResults(string regionCode)
        {
            var results = new List<Models.RaceResult>();

            //get region for gievn regionCode
            var region = entities.Regions.Where(r => r.RegionCode == regionCode ).FirstOrDefault();
            results = GetRaceResults().ToList().Where(rr => rr.RegionID == region.RegionID).ToList ();
            Mapper.CreateMap<Data.RaceResult, Models.RaceResult>();

            return results;
        }
        
        public int CreateRaceResult(Models.RaceResult raceResult)
        {
            Mapper.CreateMap<Models.RaceResult, Data.RaceResult>();
            var dataRaceResult = Mapper.Map<Models.RaceResult, Data.RaceResult>(raceResult);
            entities.AddToRaceResults(dataRaceResult);
            var created = entities.SaveChanges();
            return dataRaceResult.RaceResultID;
        }

        public bool UpdateRaceResult(Models.RaceResult raceResult)
        {
            var dataRaceResult = entities.RaceResults.SingleOrDefault(rr => rr.RaceResultID == raceResult.RaceResultID);
            if (dataRaceResult != null)
            {
                BuildDataRaceResult(raceResult, ref dataRaceResult);
                var updated = entities.SaveChanges();
                return (updated > 0);
            }
            return false;
        }

        private void BuildDataRaceResult(Models.RaceResult raceResult, ref Data.RaceResult dataRaceResult)
        {
            dataRaceResult.RaceID = raceResult.RaceID;
            dataRaceResult.NoOfVotes = raceResult.NoOfVotes;
            dataRaceResult.CandidateID = raceResult.CandidateID;
            dataRaceResult.RegionID = raceResult.RegionID;
            dataRaceResult.ModifiedBy = raceResult.ModifiedBy;
            dataRaceResult.ModifiedOn = DateTime.Now;
        }

        public bool DeleteRaceResult(int raceResultID)
        {
            var dataRaceResult = entities.RaceResults.SingleOrDefault(rr => rr.RaceResultID == raceResultID);
            if (dataRaceResult != null)
            {
                entities.RaceResults.DeleteObject(dataRaceResult);
                return true;
            }
            return false;
        }


        public IEnumerable<Models.AggregatedRaceResult> GetAggregatedRaceResults(int raceID)
        {
            var dataRaceResults = entities.GetAggregatedRaceResult(raceID);
            Mapper.CreateMap<Data.AggregatedRaceResult, Models.AggregatedRaceResult>();
            var modelRaceResults = Mapper.Map<IEnumerable<Data.AggregatedRaceResult>, IEnumerable<Models.AggregatedRaceResult>>(dataRaceResults);
            return modelRaceResults;
        }

        public IEnumerable<Models.AggregatedRaceResult> GetAggregatedRaceResults(int raceID, string regionCode)
        {
            var dataRaceResults = entities.GetAggregatedRaceResultByRegion(raceID, regionCode);
            Mapper.CreateMap<Data.AggregatedRaceResult, Models.AggregatedRaceResult>();
            var modelRaceResults = Mapper.Map<IEnumerable<Data.AggregatedRaceResult>, IEnumerable<Models.AggregatedRaceResult>>(dataRaceResults);
            return modelRaceResults;
        }


        public IEnumerable<Models.Region> GetRegions()
        {
            var dataRegions = entities.Regions;
            Mapper.CreateMap<Data.Region, Models.Region>();
            var modelRegions = Mapper.Map<IEnumerable<Data.Region>, IEnumerable<Models.Region>>(dataRegions);
            return modelRegions;
        }


        public IEnumerable<Models.RaceType> GetRaceTypes()
        {
            var dataRaceTypes = entities.RaceTypes;
            Mapper.CreateMap<Data.RaceType, Models.RaceType>();
            var modelRaceTypes = Mapper.Map<IEnumerable<Data.RaceType>, IEnumerable<Models.RaceType>>(dataRaceTypes);
            return modelRaceTypes;
        }


        public IEnumerable<Models.Candidate> GetCandidates()
        {
            var dataCandidates = entities.Candidates;
            Mapper.CreateMap<Data.Candidate, Models.Candidate>();
            var modelCandidates = Mapper.Map<IEnumerable<Data.Candidate>, IEnumerable<Models.Candidate>>(dataCandidates);
            return modelCandidates;
        }
        
        public IEnumerable<Models.Party> GetParties()
        {
            var dataParties = entities.Parties;
            Mapper.CreateMap<Data.Party, Models.Party>();
            var modelParties = Mapper.Map<IEnumerable<Data.Party>, IEnumerable<Models.Party>>(dataParties);
            return modelParties;
        }
    }
}
