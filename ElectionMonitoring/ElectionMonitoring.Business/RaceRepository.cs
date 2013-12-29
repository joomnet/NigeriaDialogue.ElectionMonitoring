using System;
using System.Collections.Generic;
using System.Linq;
using ElectionMonitoring.Data;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.Business
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ElectionMonitoringEntities entities = new ElectionMonitoringEntities();

        public IEnumerable<Race> GetRaces()
        {
            return entities.Races.ToArray();
        }

        public Race GetRace(int raceId)
        {
            return GetRaces().FirstOrDefault(r => r.RaceID == raceId);
        }

        public int CreateRace(Race race)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRace(Race race)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRace(int raceId)
        {
            throw new NotImplementedException();
        }
    }
}