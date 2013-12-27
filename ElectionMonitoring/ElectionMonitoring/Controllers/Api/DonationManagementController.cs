using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ElectionMonitoring.Business;
using AutoMapper;
using System.Text;
using System.Reflection;

namespace ElectionMonitoring.Controllers.Api
{
    public class DonationManagementController : ApiController
    {
        private IDonorRepository _donorRepository;
        private IDonationRepository _donationRepository;
        private IProjectRepository _projectRepository;
        public DonationManagementController(IDonorRepository donorRepository,
            IDonationRepository donationRepository, IProjectRepository projectRepository)
        {
            if ((donorRepository == null) ||
                (donationRepository == null) ||
                (projectRepository == null))
            {
                throw new Exception("Missing repository.");
            }
            _donorRepository = donorRepository;
            _donationRepository = donationRepository;
            _projectRepository = projectRepository;
        }

        #region Misc
        [HttpGet]
        public IEnumerable<string> About()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
            TimeSpan.TicksPerDay * version.Build + // days since 1 January 2000
            TimeSpan.TicksPerSecond * 2 * version.Revision)); // seconds since midnight, (multiply by 2 to get original)

            var returnValue = new List<string>();
            returnValue.Add("Service : Donation Management Service");
            returnValue.Add("Version : " + version.ToString());
            returnValue.Add("Build Date : " + buildDateTime.ToLongDateString() + " " + buildDateTime.ToLongTimeString());
            returnValue.Add("Data LastUpdated : 26th November 2013 16:05 GMT");
            returnValue.Add("Author : Sola Oderinde");
            returnValue.Add("Organisation : Nigeria Dialogue");

            return returnValue;
        }
        #endregion

        #region Donations
        // GET api/donationmanagement/donations
        [HttpGet]
        [ActionName("Donations")]
        public IEnumerable<Models.Donation> GetDonations()
        {
            return _donationRepository.GetDonations();
        }

        // GET api/donationmanagement/donations/5
        [HttpGet]
        [ActionName("Donations")]
        public Models.Donation GetDonation(int id)
        {
            var donation = _donationRepository.GetDonation(id);
            if (donation == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return donation;
        }

        // POST api/donationmanagement/donations
        [HttpPost]
        [ActionName("Donations")]
        public HttpResponseMessage Post(DTO.Donation donation)
        {
            if ((ModelState.IsValid) && (donation != null))
            {
                // check if known donor otherwise create new donor
                var donor = _donorRepository.GetDonor(donation.DonorID);
                if (donor == null)
                {
                    Mapper.CreateMap<DTO.Donor, Models.Donor>();
                    donor = _donorRepository.CreateDonor(Mapper.Map<DTO.Donor, Models.Donor>(donation.Donor));
                }

                if (donation.Amount == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, " Error Creating donation");
                Mapper.CreateMap<DTO.Donation, Models.Donation>();
                var modelDonation = Mapper.Map<DTO.Donation, Models.Donation>(donation);

                modelDonation.DonorID = donor.DonorID;
                modelDonation.Donor = null;
                var created = _donationRepository.CreateDonation(modelDonation);
                if ((created !=null) && (created.DonationID  > 0))
                {
                    var response = Request.CreateResponse(HttpStatusCode.Created, donation);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = donation.DonationID }));
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, " Error Creating donation");
                }

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // PUT api/donationmanagement/donations/5
        [HttpPut]
        [ActionName("Donations")]
        public HttpResponseMessage Put(int id, Models.Donation donation)
        {
            if (ModelState.IsValid && id == donation.DonationID)
            {
                var updated = _donationRepository.UpdateDonation(donation);
                if (updated)
                    return Request.CreateResponse
                        (HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotModified, "Could not modify donation");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).ToList();
                var errostring = new StringBuilder();
                foreach (var error in errors)
                {
                    var exMessage = error.Select(e => e.Exception.Message).ToList();
                    errostring.Append(exMessage);
                }

                return Request.CreateErrorResponse(HttpStatusCode.NotModified, errostring.ToString());
            }
        }

        // DELETE api/donationmanagement/donations/5
        [HttpDelete]
        [ActionName("Donations")]
        public HttpResponseMessage DeleteDonation(int id)
        {
            try
            {
                var deleted = _donationRepository.DeleteDonation(id);
                if (deleted)
                    return Request.CreateResponse
                        (HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not modify donation");
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Exception donation");
            }
        }
        #endregion

        #region Donors
        // GET api/donationmanagement/donors
        [HttpGet]
        [ActionName("Donors")]
        public IEnumerable<Models.Donor> GetDonors()
        {
            return _donorRepository.GetDonors();
        }

        // GET api/donationmanagement/donors/5
        [HttpGet]
        [ActionName("Donors")]
        public Models.Donor GetDonor(int id)
        {
            var donor = _donorRepository.GetDonor(id);
            if (donor == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return donor;
        }

        // POST api/donationmanagement/donors
        [HttpPost]
        [ActionName("Donors")]
        public HttpResponseMessage Post(Models.Donor donor)
        {
            if ((ModelState.IsValid) && (donor != null))
            {
                if (String.IsNullOrEmpty(donor.FirstName) ||
                    String.IsNullOrEmpty(donor.LastName))
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, " Error Creating Donor");
                //donor.DateCreated = DateTime.Now;
                //donor.CreatedBy = (new UserRepository().GetUser(User.Identity.Name)).UserID;
                var created = _donorRepository.CreateDonor(donor);
                if (created.DonorID > 0)
                {
                    var response = Request.CreateResponse(HttpStatusCode.Created, donor);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = donor.DonorID }));
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, " Error Creating Donor");
                }

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // PUT api/donationmanagement/donor/5
        [HttpPut]
        [ActionName("Donors")]
        public HttpResponseMessage Put(int id, Models.Donor donor)
        {
            if (ModelState.IsValid && id == donor.DonorID)
            {
                //contact.DateLastModified = DateTime.Now;
                //contact.LastModifiedBy = (new UserRepository().GetUser(User.Identity.Name)).UserID;
                var updated = _donorRepository.UpdateDonor(donor);
                if (updated)
                    return Request.CreateResponse
                        (HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotModified, "Could not modify Contact");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).ToList();
                var errostring = new StringBuilder();
                foreach (var error in errors)
                {
                    var exMessage = error.Select(e => e.Exception.Message).ToList();
                    errostring.Append(exMessage);
                }

                return Request.CreateErrorResponse(HttpStatusCode.NotModified, errostring.ToString());
            }
        }

        // DELETE api/donationmanagement/donor/5
        [HttpDelete]
        [ActionName("Donors")]
        public HttpResponseMessage DeleteDonor(int id)
        {
            try
            {
                var deleted = _donorRepository.DeleteDonor(id);
                if (deleted)
                    return Request.CreateResponse
                        (HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not modify Contact");
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Exception Occured");
            }
        }
        #endregion
        
        #region Projects
        // GET api/donationmanagement/projects
        [HttpGet]
        [ActionName("Projects")]
        public IEnumerable<Models.Project> GetProjects()
        {
            return _projectRepository.GetProjects();
        }

        // GET api/donationmanagement/projects/5
        [HttpGet]
        [ActionName("Projects")]
        public Models.Project GetProject(int id)
        {
            var project = _projectRepository.GetProject(id);
            if (project == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return project;
        }

        // POST api/donationmanagement/projects
        [HttpPost]
        [ActionName("Projects")]
        public HttpResponseMessage Post(Models.Project project)
        {
            if ((ModelState.IsValid) && (project != null))
            {
                if (String.IsNullOrEmpty(project.Title))
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, " Error Creating project");
                var created = _projectRepository.CreateProject(project);
                if (created > 0)
                {
                    var response = Request.CreateResponse(HttpStatusCode.Created, project);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = project.ProjectID }));
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, " Error Creating project");
                }

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // PUT api/donationmanagement/project/5
        [HttpPut]
        [ActionName("Projects")]
        public HttpResponseMessage Put(int id, Models.Project project)
        {
            if (ModelState.IsValid && id == project.ProjectID)
            {
                var updated = _projectRepository.UpdateProject(project);
                if (updated)
                    return Request.CreateResponse
                        (HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotModified, "Could not modify project");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors).ToList();
                var errostring = new StringBuilder();
                foreach (var error in errors)
                {
                    var exMessage = error.Select(e => e.Exception.Message).ToList();
                    errostring.Append(exMessage);
                }

                return Request.CreateErrorResponse(HttpStatusCode.NotModified, errostring.ToString());
            }
        }

        // DELETE api/donationmanagement/project/5
        [HttpDelete]
        [ActionName("Projects")]
        public HttpResponseMessage DeleteProject(int id)
        {
            try
            {
                var deleted = _projectRepository.DeleteProject(id);
                if (deleted)
                    return Request.CreateResponse
                        (HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not modify project");
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Exception project");
            }
        }
        #endregion
    }
}
