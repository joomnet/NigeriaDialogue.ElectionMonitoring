using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionMonitoring.DTO
{
    public class Party
    {
        public int PartyID { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public string LogoFile { get; set; }
        public string Color { get; set; }
    }
}