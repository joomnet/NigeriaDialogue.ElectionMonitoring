//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ElectionMonitoring.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Race
    {
        public Race()
        {
            this.RaceResults = new HashSet<RaceResult>();
        }
    
        public int RaceID { get; set; }
        public Nullable<int> RaceTypeID { get; set; }
        public Nullable<int> Year { get; set; }
        public string Description { get; set; }
    
        public virtual RaceType RaceType { get; set; }
        public virtual ICollection<RaceResult> RaceResults { get; set; }
    }
}
