using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    using ElectionMonitoring;

    public interface IRaceResultService
    {
        IEnumerable<Models.RaceResult> GetRaceResults();
        IEnumerable<Models.RaceResult> GetRaceResults(string regionCode);
        int CreateRaceResult(Models.RaceResult raceResult);
        bool UpdateRaceResult(Models.RaceResult raceResult);
        bool DeleteRaceResult(int raceResultID);

        IEnumerable<Models.AggregatedRaceResult> GetAggregatedRaceResults(int raceID);
        IEnumerable<Models.AggregatedRaceResult> GetAggregatedRaceResults(int raceID, string regionCode);

        IEnumerable<Models.Region> GetRegions();
        IEnumerable<Models.RaceType> GetRaceTypes();
        IEnumerable<Models.Candidate> GetCandidates();
        IEnumerable<Models.Party> GetParties();
    }
}
