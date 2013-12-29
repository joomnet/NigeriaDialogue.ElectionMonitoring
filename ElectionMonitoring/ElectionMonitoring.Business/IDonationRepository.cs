using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElectionMonitoring.Data;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.Business
{
    public interface IDonationRepository
    {
        IEnumerable<Donation> GetDonations();
        Donation GetDonation(int donationId);
        Donation CreateDonation(Donation donation);
        bool UpdateDonation(Donation donation);
        bool DeleteDonation(int DonationID);
    }
}
