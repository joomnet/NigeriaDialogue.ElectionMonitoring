using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ElectionMonitoring.Business;
using ElectionMonitoring.Controllers;
using ElectionMonitoring.ViewModels;
using Machine.Specifications;
using NUnit.Framework;

namespace ElectionMonitoring.UnitTests
{
    [TestFixture]
    public class ElectionResultControllerTests
    {
        private ElectionResultController elctionResultController;
        private ActionResult raceResultsPage;

        [Test]
        public void When_RaceResults_Are_Requested()
        {
            Establish context = () => elctionResultController = new ElectionResultController(new RaceResultService());

            Because of = () => raceResultsPage = elctionResultController.RaceResults();

            It should_set_the_page_title_to_results = () =>
                {
                    raceResultsPage.ShouldBeOfType<RaceResultsViewModel>();
                    ((raceResultsPage as ViewResult).Model as RaceResultsViewModel).Title.ShouldEqual("Presidential election results");
                };
        }
    }
}
