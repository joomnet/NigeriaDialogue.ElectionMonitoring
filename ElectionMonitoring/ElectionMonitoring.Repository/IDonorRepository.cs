using System.Collections.Generic;

namespace ElectionMonitoring.Repository
{
    public interface IDonorRepository
    {
        IEnumerable<Models.Donor> GetDonors();
        Models.Donor GetDonor(int donorId);
        Models.Donor CreateDonor(Models.Donor donor);
        bool UpdateDonor(Models.Donor donor);
        bool DeleteDonor(int DonorID);
    }
}
