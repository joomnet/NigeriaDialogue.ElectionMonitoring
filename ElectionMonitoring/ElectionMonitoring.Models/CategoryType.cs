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
    
    public partial class CategoryType
    {
        public CategoryType()
        {
            this.Categories = new HashSet<Category>();
        }
    
        public int CategoryTypeID { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Category> Categories { get; set; }
    }
}
