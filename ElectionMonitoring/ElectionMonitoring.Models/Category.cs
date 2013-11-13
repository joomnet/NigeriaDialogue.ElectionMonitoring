using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public bool TopLevel { get; set; }
        public int ParentCategoryID { get; set; }
        public int SortOrder { get; set; }
        public int StatusID { get; set; }
        public int CategoryTypeID { get; set; }
    }
}
