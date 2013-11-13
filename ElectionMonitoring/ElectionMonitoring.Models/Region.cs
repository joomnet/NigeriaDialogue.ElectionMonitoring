using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Models
{
    public class Region
    {
        public int RegionID { get; set; }
        public bool TopLevel { get; set; }
        public int ParentRegionID { get; set; }
        public int StatusID { get; set; }
        public string Name { get; set; }
        public int RegionTypeID { get; set; }
        public string RegionCode { get; set; }
        public string Coordinates { get; set; }
    }
}
