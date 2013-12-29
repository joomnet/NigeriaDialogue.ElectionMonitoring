using System.Collections.Generic;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.Business
{
    public interface IRaceRepository
    {
        IEnumerable<Race> GetRaces();
        Race GetRace(int raceId);
        int CreateRace(Race race);
        bool UpdateRace(Race race);
        bool DeleteRace(int raceId);
    }
}