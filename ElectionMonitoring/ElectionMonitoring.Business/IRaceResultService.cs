using System.Collections.Generic;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.Business
{
    public interface IRaceResultService
    {
        int CreateRaceResult(RaceResult raceResult);
        /*
        IEnumerable<Models.RaceResult> GetRaceResults();
        IEnumerable<Models.RaceResult> GetRaceResults(string regionCode);
        bool UpdateRaceResult(Models.RaceResult raceResult);
        bool DeleteRaceResult(int raceResultID);
        */
        IEnumerable<AggregatedRaceResult> GetAggregatedRaceResults(int raceID, string regionCode);

        IEnumerable<Region> GetRegions();
        IEnumerable<RaceType> GetRaceTypes();
        IEnumerable<Candidate> GetCandidates();
        IEnumerable<Party> GetParties();
    }
}