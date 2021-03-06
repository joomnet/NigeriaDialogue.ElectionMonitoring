﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using AutoMapper;
using ElectionMonitoring.Business;
using ElectionMonitoring.Controllers.Api;
using ElectionMonitoring.Models;
using ElectionMonitoring.Repository;
using Moq;
using NUnit.Framework;

namespace ElectionMonitoring.Tests.Controllers
{
    [TestFixture]
    public class DonationManagementControllerTest
    {
        [SetUp]
        public void SetUp()
        {
            donations = new List<Donation>
                {
                    new Donation {DonationID = 1, DonorID = 1, Amount = 23.45M, DonationDate = DateTime.Now},
                    new Donation {DonationID = 2, DonorID = 2, Amount = 120.99M, DonationDate = DateTime.Now},
                    new Donation {DonationID = 3, DonorID = 3, Amount = 83.90M, DonationDate = DateTime.Now},
                    new Donation {DonationID = 4, DonorID = 2, Amount = 56.90M, DonationDate = DateTime.Now},
                };

            donors = new List<Donor>
                {
                    new Donor
                        {
                            DonorID = 1,
                            FirstName = "Sphinx",
                            LastName = "McFadden",
                            Gender = "Male",
                            Email = "spinx@mcfadden.com"
                        },
                    new Donor
                        {
                            DonorID = 2,
                            FirstName = "Joy",
                            LastName = "Bolden",
                            Gender = "Female",
                            Email = "joy@bolden.com"
                        },
                };

            projects = new List<Project>
                {
                    new Project
                        {
                            ProjectID = 1,
                            Title = "ElectionMonitoring Application",
                            Description = "Website shwoing real tiem election results",
                            Budget = 23500.00M
                        },
                    new Project
                        {
                            ProjectID = 2,
                            Title = "Another Project",
                            Description = "Another Project Description",
                            Budget = 13500.00M
                        },
                    new Project
                        {
                            ProjectID = 3,
                            Title = "Yet another project",
                            Description = "Yet another project Description",
                            Budget = 65000.00M
                        },
                    new Project
                        {
                            ProjectID = 4,
                            Title = "Still another project",
                            Description = "Still another project Description",
                            Budget = 10500.00M
                        },
                };

            mockDonationRepo = new Mock<IDonationRepository>();
            mockDonationRepo.Setup(d => d.GetDonations()).Returns(donations);
            mockDonationRepo.Setup(d => d.GetDonation(It.IsAny<int>()))
                            .Returns(donations.FirstOrDefault(don => don.DonorID == 2));
            mockDonationRepo.Setup(d => d.GetDonation(It.IsAny<int>()))
                            .Returns(donations.FirstOrDefault(don => don.DonorID == 2));
            mockDonationRepo.Setup(d => d.CreateDonation(It.IsAny<Donation>()))
                            .Returns(donations.FirstOrDefault(don => don.DonorID == 2));
            mockDonationRepo.Setup(d => d.UpdateDonation(It.IsAny<Donation>())).Returns(true);
            mockDonationRepo.Setup(d => d.DeleteDonation(It.IsAny<int>())).Returns(true);

            mockDonorRepo = new Mock<IDonorRepository>();
            mockDonorRepo.Setup(d => d.GetDonors()).Returns(donors);
            mockDonorRepo.Setup(d => d.GetDonor(It.IsAny<int>()))
                         .Returns(donors.FirstOrDefault(don => don.DonorID == 1));
            mockDonorRepo.Setup(d => d.CreateDonor(It.IsAny<Donor>()))
                         .Returns(donors.FirstOrDefault(don => don.DonorID == 1));
            mockDonorRepo.Setup(d => d.UpdateDonor(It.IsAny<Donor>())).Returns(true);
            mockDonorRepo.Setup(d => d.DeleteDonor(It.IsAny<int>())).Returns(true);

            mockProjectRepo = new Mock<IProjectRepository>();
            mockProjectRepo.Setup(d => d.GetProjects()).Returns(projects);
            mockProjectRepo.Setup(d => d.GetProject(It.IsAny<int>()))
                           .Returns(projects.FirstOrDefault(prj => prj.ProjectID == 1));
            mockProjectRepo.Setup(d => d.CreateProject(It.IsAny<Project>())).Returns(1);
            mockProjectRepo.Setup(d => d.UpdateProject(It.IsAny<Project>())).Returns(true);
            mockProjectRepo.Setup(d => d.DeleteProject(It.IsAny<int>())).Returns(true);
        }

        private IEnumerable<Donor> donors;
        private IEnumerable<Donation> donations;
        private IEnumerable<Project> projects;
        private Mock<IDonationRepository> mockDonationRepo;
        private Mock<IDonorRepository> mockDonorRepo;
        private Mock<IProjectRepository> mockProjectRepo;

        public DonationManagementController SetupControllerForTests(Mock<IDonorRepository> donorRepo,
                                                                    Mock<IDonationRepository> donationRepo,
                                                                    Mock<IProjectRepository> projectRepo)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/donationmanagement");
            IHttpRoute route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary {{"controller", "donationmanagement"}});
            var controller = new DonationManagementController(donorRepo.Object, donationRepo.Object, projectRepo.Object)
                {
                    ControllerContext = new HttpControllerContext(config, routeData, request),
                    Request = request
                };
            controller.Request.Properties.Add(HttpPropertyKeys.HttpRouteDataKey, routeData);
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            return controller;
        }

        [Test]
        public void DeleteDonation_Calls_Repository_Delete()
        {
            int itemToDelete = 1;
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            controller.DeleteDonation(itemToDelete);

            mockDonationRepo.Verify(x => x.DeleteDonation(itemToDelete), Times.Once(),
                                    "Controller should have called Delete on repository");
        }

        [Test]
        public void DeleteDonor_Calls_Repository_Delete()
        {
            int itemToDelete = 1;
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            controller.DeleteDonor(itemToDelete);

            mockDonorRepo.Verify(x => x.DeleteDonor(itemToDelete), Times.Once(),
                                 "Controller should have called Delete on repository");
        }

        [Test]
        public void DeleteProject_Calls_Repository_Delete()
        {
            int itemToDelete = 1;
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            controller.DeleteProject(itemToDelete);

            mockProjectRepo.Verify(x => x.DeleteProject(itemToDelete), Times.Once(),
                                   "Controller should have called Delete on repository");
        }

        [Test]
        public void GetDonation_Action_Returns_Valid_Donation_Object()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            // Act
            Donation result = controller.GetDonation(1);

            // Assert
            Assert.IsInstanceOfType(typeof (Donation), result,
                                    "The Controller.GetDonation(1) return is not of type Models.Donation");
            Assert.IsNotNull(result, "The Controller.GetDonation(1) return is null");
        }

        [Test]
        public void GetDonations_Action_Returns_Valid_IEnumerable_Of_Donations()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            // Act
            IEnumerable<Donation> result = controller.GetDonations();
            List<Donation> res = result.ToList();

            // Assert
            Assert.IsInstanceOfType(typeof (IEnumerable<Donation>), result,
                                    "The Controller.GetDonations() return is not of type IEnumerable<Donation>");
            Assert.IsNotNull(result, "The Controller.GetDonations() return is null");
            Assert.AreEqual(donations.Count(), res.Count(), string.Format(
                "The Controller.GetContacts() return count is {0} it's supposed to be {1} (not correct)",
                res.Count(), donations.Count()));
        }

        [Test]
        public void GetDonor_Action_Returns_Valid_Donor_Object()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            // Act
            Donor result = controller.GetDonor(1);

            // Assert
            Assert.IsInstanceOfType(typeof (Donor), result,
                                    "The Controller.GetDonor(1) return is not of type Models.Donor");
            Assert.IsNotNull(result, "The Controller.GetDonor(1) return is null");
        }

        [Test]
        public void GetDonors_Action_Returns_Valid_IEnumerable_Of_Donors()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            // Act
            IEnumerable<Donor> result = controller.GetDonors().ToArray();


            // Assert
            //Assert.IsInstanceOfType(typeof (IEnumerable<Donor>), result,
            //                        "The Controller.GetDonors() return is not of type IEnumerable<Donor>");
            //Assert.IsNotNull(result, "The Controller.GetDonors() return is null");
            Assert.AreEqual(donors.Count(), result.Count(), string.Format(
                "The Controller.GetDonors() return count is {0} it's supposed to be {1} (not correct)",
                result.Count(), donors.Count()));
        }

        [Test]
        public void GetProject_Action_Returns_Valid_Project_Object()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            // Act
            Project result = controller.GetProject(1);

            // Assert
            Assert.IsInstanceOfType(typeof (Project), result,
                                    "The Controller.GetProject(1) return is not of type Models.Project");
            Assert.IsNotNull(result, "The Controller.GetProject(1) return is null");
        }

        [Test]
        public void GetProjects_Action_Returns_Valid_IEnumerable_Of_Projects()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            // Act
            IEnumerable<Project> result = controller.GetProjects();
            List<Project> res = result.ToList();

            // Assert
            Assert.IsInstanceOfType(typeof (IEnumerable<Project>), result,
                                    "The Controller.GetProjects() return is not of type IEnumerable<Project>");
            Assert.IsNotNull(result, "The Controller.GetProjects() return is null");
            Assert.AreEqual(projects.Count(), res.Count(), string.Format(
                "The Controller.GetProjects() return count is {0} it's supposed to be {1} (not correct)",
                res.Count(), projects.Count()));
        }

        [TearDown]
        public void SetDown()
        {
            donors = null;
            donations = null;
            projects = null;
            mockDonationRepo = null;
            mockDonorRepo = null;
            mockProjectRepo = null;
        }

        [Test]
        public void The_PostDonation_Action_returns_created_statuscode()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);



            HttpResponseMessage result = controller.Post(donations.SingleOrDefault(u => u.DonationID == 1));

            // Assert
            Assert.IsNotNull(result, "Should have returned a HttpResponseMessage");
            Assert.IsTrue(result.IsSuccessStatusCode, "Status Code Should be success");
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Invalid status code");
        }

        [Test]
        public void The_PostDonor_Action_returns_created_statuscode()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            // Act
            HttpResponseMessage result = controller.Post(donations.SingleOrDefault(u => u.DonorID == 1));

            // Assert
            Assert.IsNotNull(result, "Should have returned a HttpResponseMessage");
            Assert.IsTrue(result.IsSuccessStatusCode, "Status Code Should be success");
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Invalid status code");
        }

        [Test]
        public void The_PostProject_Action_returns_created_statuscode()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);

            // Act
            HttpResponseMessage result = controller.Post(projects.SingleOrDefault(u => u.ProjectID == 1));

            // Assert
            Assert.IsNotNull(result, "Should have returned a HttpResponseMessage");
            Assert.IsTrue(result.IsSuccessStatusCode, "Status Code Should be success");
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Invalid status code");
        }

        [Test]
        public void The_PuProject_Action_returns_NotFound_statuscode_When_The_Project_Does_NOT_Exists()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);
            Project project = projects.SingleOrDefault(u => u.ProjectID == 2);
            int fakeId = 345;
            // Act
            HttpResponseMessage result = controller.Put(fakeId, project);

            // Assert
            Assert.IsInstanceOfType(typeof (HttpResponseMessage), result, "Should have returned an HttpResponseMessage");
            Assert.IsNotNull(result, "Should have returned a HttpResponseMessage");
            Assert.IsFalse(result.IsSuccessStatusCode, "Status Code Should be success");
            Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "Invalid status code");
        }

        [Test]
        public void The_PutDonation_Action_returns_NotFound_statuscode_When_The_Donation_Does_NOT_Exists()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);
            Donation donation = donations.SingleOrDefault(u => u.DonationID == 2);
            int fakeId = 876;
            // Act
            HttpResponseMessage result = controller.Put(fakeId, donation);

            // Assert
            Assert.IsInstanceOfType(typeof (HttpResponseMessage), result, "Should have returned an HttpResponseMessage");
            Assert.IsNotNull(result, "Should have returned a HttpResponseMessage");
            Assert.IsFalse(result.IsSuccessStatusCode, "Status Code Should be success");
            Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "Invalid status code");
        }

        [Test]
        public void The_PutDonation_Action_returns_Ok_statuscode_When_The_Donation_Exists()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);
            Donation donation = donations.SingleOrDefault(u => u.DonationID == 2);

            // Act
            HttpResponseMessage result = controller.Put(2, donation);

            // Assert
            Assert.IsNotNull(result, "Should have returned a HttpResponseMessage");
            Assert.IsTrue(result.IsSuccessStatusCode, "Status Code Should be success");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, "Invalid status code");
        }

        [Test]
        public void The_PutDonor_Action_returns_NotFound_statuscode_When_The_Donor_Does_NOT_Exists()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);
            Donor donor = donors.SingleOrDefault(u => u.DonorID == 2);
            int fakeId = 345;
            // Act
            HttpResponseMessage result = controller.Put(fakeId, donor);

            // Assert
            Assert.IsInstanceOfType(typeof (HttpResponseMessage), result, "Should have returned an HttpResponseMessage");
            Assert.IsNotNull(result, "Should have returned a HttpResponseMessage");
            Assert.IsFalse(result.IsSuccessStatusCode, "Status Code Should be success");
            Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode, "Invalid status code");
        }

        [Test]
        public void The_PutDonor_Action_returns_Ok_statuscode_When_The_Donor_Exists()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);
            Donor donor = donors.SingleOrDefault(u => u.DonorID == 2);

            // Act
            HttpResponseMessage result = controller.Put(2, donor);

            // Assert
            Assert.IsNotNull(result, "Should have returned a HttpResponseMessage");
            Assert.IsTrue(result.IsSuccessStatusCode, "Status Code Should be success");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, "Invalid status code");
        }

        [Test]
        public void The_PutProject_Action_returns_Ok_statuscode_When_The_Project_Exists()
        {
            // Arrange
            DonationManagementController controller = SetupControllerForTests(mockDonorRepo, mockDonationRepo,
                                                                              mockProjectRepo);
            Project project = projects.SingleOrDefault(u => u.ProjectID == 2);

            // Act
            HttpResponseMessage result = controller.Put(2, project);

            // Assert
            Assert.IsNotNull(result, "Should have returned a HttpResponseMessage");
            Assert.IsTrue(result.IsSuccessStatusCode, "Status Code Should be success");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, "Invalid status code");
        }
    }
}