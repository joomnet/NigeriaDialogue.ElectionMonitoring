using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using AutoMapper;
using ElectionMonitoring.Data;
using Donation = ElectionMonitoring.Models.Donation;

namespace ElectionMonitoring.Business
{
    public class DonationRepository : IDonationRepository
    {
        private readonly ElectionMonitoringEntities entities = new ElectionMonitoringEntities();

        public IEnumerable<Donation> GetDonations()
        {
            ObjectSet<Data.Donation> dataDonations = entities.Donations;
            Mapper.CreateMap<Data.Donation, Donation>();
            IEnumerable<Donation> modelDonations =
                Mapper.Map<IEnumerable<Data.Donation>, IEnumerable<Donation>>(dataDonations);
            return modelDonations;
        }

        public Donation GetDonation(int DonationID)
        {
            Data.Donation dataDonation = entities.Donations.Where(d => d.DonationID == DonationID).FirstOrDefault();
            Mapper.CreateMap<Data.Donation, Donation>();
            Donation modelDonation = Mapper.Map<Data.Donation, Donation>(dataDonation);
            return modelDonation;
        }

        public Donation CreateDonation(Donation donation)
        {
            Mapper.CreateMap<Donation, Data.Donation>();
            Data.Donation dataDonation = Mapper.Map<Donation, Data.Donation>(donation);
            entities.AddToDonations(dataDonation);
            int newID = entities.SaveChanges();
            Data.Donation dw = entities.Donations.Where(d => d.DonationID == newID).FirstOrDefault();
            Data.Donation don = dw;
            Mapper.CreateMap<Data.Donation, Donation>();
            donation = Mapper.Map<Data.Donation, Donation>(dataDonation);
            return donation;
        }

        public bool UpdateDonation(Donation donation)
        {
            Data.Donation dataDonation = entities.Donations.SingleOrDefault(d => d.DonationID == donation.DonationID);
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
            Data.Donation dataDonation = entities.Donations.SingleOrDefault(d => d.DonationID == DonationID);
            if (dataDonation != null)
            {
                entities.Donations.DeleteObject(dataDonation);
                return true;
            }
            return false;
        }
    }
}