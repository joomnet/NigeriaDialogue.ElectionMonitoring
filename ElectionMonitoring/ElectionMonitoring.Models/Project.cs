using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Models
{
    public class Project
    {
        public int ProjectID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Budget { get; set; }
    }
}
