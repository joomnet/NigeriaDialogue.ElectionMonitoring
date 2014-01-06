using System.Collections.Generic;

namespace ElectionMonitoring.Repository
{
    public interface IRegionRepository
    {
        IEnumerable<Models.Region> GetRegions();
        IEnumerable<Models.Region> GetRegions(int regionID);
        int CreateRegion(Models.Region region);
        bool UpdateRegion(Models.Region region);
        bool DeleteRegion(int regionID);

    }
}
