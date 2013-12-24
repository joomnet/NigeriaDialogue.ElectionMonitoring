using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    using ElectionMonitoring.Models;
    using AutoMapper;

    public class DonationRepository : IDonationRepository
    {
        private Data.ElectionMonitoringEntities entities = new Data.ElectionMonitoringEntities();

        public IEnumerable<Models.Donation> GetDonations()
        {
            var dataDonations = entities.Donations;
            Mapper.CreateMap<Data.Donation, Models.Donation>();
            var modelDonations = Mapper.Map<IEnumerable<Data.Donation>, IEnumerable<Models.Donation>>(dataDonations);
            return modelDonations;
        }

        public Models.Donation GetDonation(int DonationID)
        {
            var dataDonation = entities.Donations.Where(d => d.DonationID == DonationID).FirstOrDefault();
            Mapper.CreateMap<Data.Donation, Models.Donation>();
            var modelDonation = Mapper.Map<Data.Donation, Models.Donation>(dataDonation);
            return modelDonation;
        }

        public Models.Donation CreateDonation(Models.Donation donation)
        {
            Mapper.CreateMap<Models.Donation, Data.Donation>();
            var dataDonation = Mapper.Map<Models.Donation, Data.Donation>(donation);
            entities.AddToDonations(dataDonation);
            var newID = entities.SaveChanges();
            var dw = entities.Donations.Where(d => d.DonationID == newID).FirstOrDefault();
            var don = dw;
            Mapper.CreateMap<Data.Donation, Models.Donation>();
            donation = Mapper.Map<Data.Donation, Models.Donation>(dataDonation);
            return donation;
        }

        public bool UpdateDonation(Models.Donation donation)
        {
            var dataDonation = entities.Donations.SingleOrDefault(d => d.DonationID == donation.DonationID);
            if (dataDonation != null)
            {
                dataDonation.Amount = donation.Amount;
                dataDonation.DonorID = donation.DonorID;
                
                var updated = entities.SaveChanges();
                return (updated > 0);
            }
            return false;
        }

        public bool DeleteDonation(int DonationID)
        {
            var dataDonation = entities.Donations.SingleOrDefault(d => d.DonationID == DonationID);
            if (dataDonation != null)
            {
                entities.Donations.DeleteObject(dataDonation);
                return true;
            }
            return false;
        }
    }
}
