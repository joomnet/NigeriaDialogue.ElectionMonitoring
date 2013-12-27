using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    using AutoMapper;

    public class RaceRepository : IRaceRepository
    {
        Data.ElectionMonitoringEntities entities = new Data.ElectionMonitoringEntities();

        public IEnumerable<Models.Race> GetRaces()
        {
            var dataRaces = entities.Races;
            Mapper.CreateMap<Data.Race, Models.Race>();
            var modelRaces = Mapper.Map<IEnumerable<Data.Race>, IEnumerable<Models.Race>>(dataRaces);
            return modelRaces;
        }

        public Models.Race GetRace(int raceID)
        {
            return GetRaces().Where(r => r.RaceID == raceID).FirstOrDefault();
        }

        public int CreateRace(Models.Race race)
        {
            Mapper.CreateMap<Models.Race, Data.Race>();
            var dataRace = Mapper.Map<Models.Race, Data.Race>(race);
            entities.AddToRaces(dataRace);
            var created = entities.SaveChanges();
            return dataRace.RaceID;
        }

        public bool UpdateRace(Models.Race race)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRace(int raceID)
        {
            throw new NotImplementedException();
        }
    }
}
