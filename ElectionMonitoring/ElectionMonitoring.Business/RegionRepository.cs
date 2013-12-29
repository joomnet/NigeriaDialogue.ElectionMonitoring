using System;
using System.Collections.Generic;
using System.Linq;
using ElectionMonitoring.Data;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.Business
{
    public class RegionRepository : IRegionRepository
    {
        private readonly ElectionMonitoringEntities entities = new ElectionMonitoringEntities();

        public IEnumerable<Region> GetRegions()
        {
            return entities.Regions.ToArray();
        }

        public IEnumerable<Region> GetRegions(int regionID)
        {
            return GetRegions().Where(r => r.RegionID == regionID).ToList();
        }

        public int CreateRegion(Region region)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRegion(Region region)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRegion(int regionID)
        {
            throw new NotImplementedException();
        }
    }
}