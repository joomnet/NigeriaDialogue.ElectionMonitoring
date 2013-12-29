using System.Collections.Generic;
using System.Linq;
using ElectionMonitoring.Data;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.Business
{
    public class RaceResultService : IRaceResultService
    {
        private readonly ElectionMonitoringEntities entities = new ElectionMonitoringEntities();

        #region No LONGER IN USE

        public int CreateRaceResult(RaceResult raceResult)
        {
            entities.RaceResults.Add(raceResult);
            int created = entities.SaveChanges();
            return raceResult.RaceResultID;
        }
        #endregion

        public IEnumerable<AggregatedRaceResult> GetAggregatedRaceResults(int raceID, string regionCode)
        {
            return entities.GetAggregatedRaceResult(raceID, regionCode);
        }


        public IEnumerable<Region> GetRegions()
        {
            return entities.Regions.AsEnumerable();
        }


        public IEnumerable<RaceType> GetRaceTypes()
        {
            return entities.RaceTypes.AsEnumerable();
        }


        public IEnumerable<Candidate> GetCandidates()
        {
            return entities.Candidates.AsEnumerable();
        }

        public IEnumerable<Party> GetParties()
        {
            return entities.Parties.AsEnumerable();
        }
    }
}