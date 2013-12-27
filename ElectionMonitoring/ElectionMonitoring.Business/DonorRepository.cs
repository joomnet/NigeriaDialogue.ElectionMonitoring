using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    using ElectionMonitoring.Models;
    using AutoMapper;

    public class DonorRepository : IDonorRepository
    {
        private readonly Data.ElectionMonitoringEntities entities = new Data.ElectionMonitoringEntities();

        public IEnumerable<Donor> GetDonors()
        {
            var dataDonors = entities.Donors;
            Mapper.CreateMap<Data.Donor, Models.Donor>();
            var modelDonors = Mapper.Map<IEnumerable<Data.Donor>, IEnumerable<Models.Donor>>(dataDonors);
            return modelDonors;
        }

        public Models.Donor GetDonor(int DonorID)
        {
            var dataDonor = entities.Donors.SingleOrDefault(d => d.DonorID == DonorID);
            Mapper.CreateMap<Data.Donor, Models.Donor>();
            var modelDonor = Mapper.Map<Data.Donor, Models.Donor>(dataDonor);
            return modelDonor;
        }

        public Models.Donor CreateDonor(Models.Donor donor)
        {
            Mapper.CreateMap<Models.Donor, Data.Donor>();
            var dataDonor = Mapper.Map<Models.Donor, Data.Donor>(donor);
            entities.AddToDonors(dataDonor);
            var newID = entities.SaveChanges();
            var dw = entities.Donors.Where(d => d.DonorID == newID).FirstOrDefault();
            var de = dw;
            Mapper.CreateMap<Data.Donor, Models.Donor>();
            donor = Mapper.Map<Data.Donor, Models.Donor>(dataDonor);
            return donor;
        }

        public bool UpdateDonor(Models.Donor donor)
        {
            var dataDonor = entities.Donors.SingleOrDefault(d => d.DonorID == donor.DonorID);
            if (dataDonor != null)
            {
                dataDonor.Email = donor.Email;
                dataDonor.FirstName = donor.FirstName;
                dataDonor.LastName = donor.LastName;

                var updated = entities.SaveChanges();
                return (updated > 0);
            }
            return false;
        }

        public bool DeleteDonor(int DonorID)
        {
            var dataDonor = entities.Donors.SingleOrDefault(d => d.DonorID == DonorID);
            if (dataDonor != null)
            {
                entities.Donors.DeleteObject(dataDonor);
                return true;
            }
            return false;
        }
    }
}
