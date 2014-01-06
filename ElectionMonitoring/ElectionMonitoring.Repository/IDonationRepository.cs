using System.Collections.Generic;
using ElectionMonitoring.Models;

namespace ElectionMonitoring.Repository
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
