using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    using AutoMapper;

    public class RegionRepository : IRegionRepository
    {
        private Data.ElectionMonitoringEntities entities = new Data.ElectionMonitoringEntities();
        public RegionRepository()
        {
        }

        public IEnumerable<Models.Region> GetRegions()
        {
            var dataRegions = entities.Regions;
            Mapper.CreateMap<Data.Region, Models.Region>();
            var modelRegions = Mapper.Map<IEnumerable<Data.Region>, IEnumerable<Models.Region>>(dataRegions);
            return modelRegions;
        }

        public IEnumerable<Models.Region> GetRegions(int regionID)
        {
            return GetRegions().Where(r => r.RegionID == regionID).ToList();
        }

        public int CreateRegion(Models.Region region)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRegion(Models.Region region)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRegion(int regionID)
        {
            throw new NotImplementedException();
        }
    }
}
