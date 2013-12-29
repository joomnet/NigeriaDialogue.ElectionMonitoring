using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using ElectionMonitoring.Business;
using ElectionMonitoring.Controllers.Api;
using ElectionMonitoring.Models;
using Moq;
using NUnit.Framework;

namespace ElectionMonitoring.Tests.Controllers
{
    [TestFixture]
    public class ElectionMonitoringControllerTest
    {
        [SetUp]
        public void SetUp()
        {
            races = new List<Race>
                {
                    new Race {RaceID = 1, RaceTypeID = 1, Year = 2013},
                    new Race {RaceID = 2, RaceTypeID = 2, Year = 2013},
                    new Race {RaceID = 3, RaceTypeID = 2, Year = 2013},
                };

            raceResults = new List<AggregatedRaceResult>
                {
                    new AggregatedRaceResult
                        {
                            CandidateId = 1,
                            FirstName = "Abba",
                            LastName = "Don",
                            PartyAcronym = "AD",
                            RaceID = 1,
                            TotalVotes = 2554544,
                            RegionCode = "AB",
                            PartyName = "Alliance for Democracy"
                        },
                    new AggregatedRaceResult
                        {
                            CandidateId = 2,
                            FirstName = "Phil",
                            LastName = "Peter",
                            PartyAcronym = "PDP",
                            RaceID = 1,
                            TotalVotes = 36345,
                            RegionCode = "AB",
                            PartyName = "People's Democratic Party"
                        },
                };
            regions = new List<Region>
                {
                    new Region {RegionID = 1, ParentRegionID = 0, RegionCode = "AB", Name = "Abia", TopLevel = true},
                    new Region
                        {
                            RegionID = 38,
                            ParentRegionID = 1,
                            RegionCode = "AB.AS",
                            Name = "Aba South",
                            TopLevel = false
                        },
                    new Region
                        {
                            RegionID = 39,
                            ParentRegionID = 1,
                            RegionCode = "AB.AN",
                            Name = "Aba North",
                            TopLevel = false
                        },
                    new Region {RegionID = 37, ParentRegionID = 0, RegionCode = "ZA", Name = "Zamfara", TopLevel = true},
                    new Region
                        {
                            RegionID = 700,
                            ParentRegionID = 37,
                            RegionCode = "ZA.MA",
                            Name = "Malif",
                            TopLevel = false
                        },
                };
            mockRaceRepo = new Mock<IRaceRepository>();
            mockRaceRepo.Setup(r => r.GetRace(It.IsAny<int>())).Returns(races.FirstOrDefault());
            mockRaceRepo.Setup(r => r.GetRaces()).Returns(races);

            mockRegionRepo = new Mock<IRegionRepository>();
            mockRegionRepo.Setup(r => r.GetRegions(It.IsAny<int>()))
                          .Returns(regions.Where(re => re.ParentRegionID == 1).ToList());
            mockRegionRepo.Setup(r => r.GetRegions()).Returns(regions);

            mockRaceResultRepo = new Mock<IRaceResultService>();
            mockRaceResultRepo.Setup(r => r.GetAggregatedRaceResults(It.IsAny<int>(), It.IsAny<string>()))
                              .Returns(raceResults);

            controller = SetupControllerForTests(mockRaceResultRepo, mockRegionRepo, mockRaceRepo);
        }

        [TearDown]
        public void TearDown()
        {
            raceResults = null;
            regions = null;
        }

        private IEnumerable<AggregatedRaceResult> raceResults;
        private Mock<IRaceRepository> mockRaceRepo;
        private Mock<IRaceResultService> mockRaceResultRepo;
        private Mock<IRegionRepository> mockRegionRepo;
        private IEnumerable<Region> regions;
        private IEnumerable<Race> races;
        private ElectionMonitoringController controller;

        public ElectionMonitoringController SetupControllerForTests(Mock<IRaceResultService> mockRaceResultRepo,
                                                                    Mock<IRegionRepository> mockRegionRepo,
                                                                    Mock<IRaceRepository> mockRaceRepo)
        {
            var controller = new ElectionMonitoringController(mockRaceResultRepo.Object, mockRegionRepo.Object,
                                                              mockRaceRepo.Object);
            var config = new HttpConfiguration();
            IHttpRoute route = config.Routes.MapHttpRoute("ActionApi", "api/{controller}/{action}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary {{"controller", "electionmonitoring"}});
            var request = new HttpRequestMessage();
            request.Properties.Add(HttpPropertyKeys.HttpRouteDataKey, routeData);
            request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            return controller;
        }

        [Test]
        public void GetRaces_Call_Repository()
        {
            // Arrange
            controller.Request.Method = HttpMethod.Post;

            // Act
            IEnumerable<Race> races = controller.GetRaces();

            // Assert
            Assert.IsNotNull(races, "GetRaces() should not be null");
            Assert.IsInstanceOfType(typeof (IEnumerable<Race>), races,
                                    "GetRaces() should return an IEnumerable<Models.Race>");
            Assert.IsTrue(races.FirstOrDefault().RaceID == 1, "Should return 1 for Race ID ");
        }

        [Test]
        public void GetRegions_Call_Repository()
        {
            // Arrange
            controller.Request.Method = HttpMethod.Post;

            // Act
            IEnumerable<Region> region = controller.GetRegions();

            // Assert
            Assert.IsInstanceOfType(typeof (IEnumerable<Region>), region, "Should return IEnumerable<Region>");
            Assert.IsNotNull(region, "Should return a list of region");
            Assert.AreEqual(region.FirstOrDefault().Name, "Abia", "Should return Abia as name of first region");
        }

        [Test]
        public void GetSubRegions_Call_Repository()
        {
            // Arrange
            controller.Request.Method = HttpMethod.Post;

            // Act
            IEnumerable<Region> region = controller.GetSubRegions(1);

            // Assert
            Assert.IsInstanceOfType(typeof (IEnumerable<Region>), region, "Should return IEnumerable<Region>");
            //mockRegionRepo.Verify(r => r.GetRegions(It.IsAny<int>()), Times.AtLeastOnce(), "Should have called GetRegions(1) on RegionRepository");
            Assert.IsNotNull(region, "Should return a list of region");
            Assert.AreEqual(region.FirstOrDefault().Name, "Aba South", "Should return Abia as name of first region");
        }

        //[Test]
        //public void GetUsers_Action_Returns_Valid_IEnumerable_Of_Users()
        //{
        //    // Arrange
        //    var usersController = SetupControllerForTests(mockRepo);

        //    // Act
        //    var result = usersController.GetUsers();
        //    var res = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOfType(result, typeof(IEnumerable<User>),
        //               "The usersController.GetUsers() return is not of type IEnumerable<User>");
        //    Assert.IsNotNull(result, "The usersController.GetUsers() return is null");
        //    Assert.AreEqual(users.Count(), res.Count(), string.Format(
        //        "The usersController.GetUsers() return count is {0} it's supposed to be {1} (not correct)",
        //        res.Count(), users.Count()));
        //}

        //[Test]
        //public void GetUser_Action_Returns_Valid_User_Object()
        //{
        //    // Arrange
        //    var usersController = SetupControllerForTests(mockRepo);

        //    // Act
        //    var result = usersController.GetUser(1);

        //    // Assert
        //    Assert.IsInstanceOfType(result, typeof(UserDTO),
        //               "The usersController.GetUser() return is not of type UserDTO");
        //    Assert.IsNotNull(result, "The usersController.GetUser(1) return is null");
        //}
    }
}