using System.Collections.Generic;
using System.Linq;
using ElectionMonitoring.Data;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.Business
{
    public class DonorRepository : IDonorRepository
    {
        private readonly ElectionMonitoringEntities entities = new ElectionMonitoringEntities();

        public IEnumerable<Donor> GetDonors()
        {
            return entities.Donors;
        }

        public Donor GetDonor(int donorId)
        {
            return entities.Donors.FirstOrDefault(d => d.DonorID == donorId);
        }

        public Donor CreateDonor(Donor donor)
        {
            entities.Donors.Add(donor);
            int newID = entities.SaveChanges();
            return donor;
        }

        public bool UpdateDonor(Donor donor)
        {
            Donor dataDonor = entities.Donors.SingleOrDefault(d => d.DonorID == donor.DonorID);
            if (dataDonor != null)
            {
                dataDonor.Email = donor.Email;
                dataDonor.FirstName = donor.FirstName;
                dataDonor.LastName = donor.LastName;

                int updated = entities.SaveChanges();
                return (updated > 0);
            }
            return false;
        }

        public bool DeleteDonor(int DonorID)
        {
            Donor dataDonor = entities.Donors.SingleOrDefault(d => d.DonorID == DonorID);
            if (dataDonor != null)
            {
                entities.Donors.Remove(dataDonor);
                entities.SaveChanges();
                return true;
            }
            return false;
        }
    }
}