using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    public interface IRaceRepository
    {
        IEnumerable<Models.Race> GetRaces();
        Models.Race GetRace(int raceID);
        int CreateRace(Models.Race race);
        bool UpdateRace(Models.Race race);
        bool DeleteRace(int raceID);
    }
}
