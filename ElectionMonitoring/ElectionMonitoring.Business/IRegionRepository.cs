using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
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
