using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElectionMonitoring.Data;

namespace ElectionMonitoring.Business
{
    public interface IDonorRepository
    {
        IEnumerable<Models.Donor> GetDonors();
        Models.Donor GetDonor(int DonorID);
        Models.Donor CreateDonor(Models.Donor donor);
        bool UpdateDonor(Models.Donor donor);
        bool DeleteDonor(int DonorID);
    }
}
