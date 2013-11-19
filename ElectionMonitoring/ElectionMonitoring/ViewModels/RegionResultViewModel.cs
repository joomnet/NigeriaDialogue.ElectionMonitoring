using System.Collections.Generic;

namespace ElectionMonitoring.ViewModels
{
    public class RegionResultViewModel
    {
        public RegionViewModel Region { get; set; }
        public List<AggregatedRaceResultViewModel> Results { get; set; }
        public AggregatedRaceResultViewModel Winner { get; set; }
    }
}