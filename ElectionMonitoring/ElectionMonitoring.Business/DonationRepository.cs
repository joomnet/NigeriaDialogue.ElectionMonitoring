using System.Collections.Generic;
using System.Linq;
using ElectionMonitoring.Data;
using ElectionMonitoring.Models;


namespace ElectionMonitoring.Business
{
    public class DonationRepository : IDonationRepository
    {
        private readonly ElectionMonitoringEntities entities = new ElectionMonitoringEntities();

        public IEnumerable<Donation> GetDonations()
        {
            return entities.Donations.ToArray();
        }

        public Donation GetDonation(int donationId)
        {
            return entities.Donations.FirstOrDefault(x => x.DonationID == donationId);
        }

        public Donation CreateDonation(Donation donation)
        {
            entities.Donations.Add(donation);
            entities.SaveChanges();
            return donation;
        }

        public bool UpdateDonation(Donation donation)
        {
            Donation dataDonation = entities.Donations.SingleOrDefault(d => d.DonationID == donation.DonationID);
            if (dataDonation != null)
            {
                dataDonation.Amount = donation.Amount;
                dataDonation.DonorID = donation.DonorID;

                int updated = entities.SaveChanges();
                return (updated > 0);
            }
            return false;
        }

        public bool DeleteDonation(int DonationID)
        {
            Donation dataDonation = entities.Donations.SingleOrDefault(d => d.DonationID == DonationID);
            if (dataDonation != null)
            {
                entities.Donations.Remove(dataDonation);
                return true;
            }
            return false;
        }
    }
}