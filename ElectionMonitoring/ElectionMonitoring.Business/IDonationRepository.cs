using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    public interface IDonationRepository
    {
        IEnumerable<Models.Donation> GetDonations();
        Models.Donation GetDonation(int DonationID);
        Models.Donation CreateDonation(Models.Donation donation);
        bool UpdateDonation(Models.Donation donation);
        bool DeleteDonation(int DonationID);
    }
}
