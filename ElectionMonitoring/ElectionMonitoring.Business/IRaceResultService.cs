using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    public interface IRaceResultService
    {
        int CreateRaceResult(Models.RaceResult raceResult);
        /*
        IEnumerable<Models.RaceResult> GetRaceResults();
        IEnumerable<Models.RaceResult> GetRaceResults(string regionCode);
        bool UpdateRaceResult(Models.RaceResult raceResult);
        bool DeleteRaceResult(int raceResultID);
        */
        IEnumerable<Models.AggregatedRaceResult> GetAggregatedRaceResults(int raceID, string regionCode);

        IEnumerable<Models.Region> GetRegions();
        IEnumerable<Models.RaceType> GetRaceTypes();
        IEnumerable<Models.Candidate> GetCandidates();
        IEnumerable<Models.Party> GetParties();
    }
}
