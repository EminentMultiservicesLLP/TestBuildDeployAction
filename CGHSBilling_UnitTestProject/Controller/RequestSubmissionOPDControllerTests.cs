using NUnit.Framework;
using Moq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CGHSBilling.Controllers;
using System.Collections.Generic;
using CGHSBilling.Models;
using System.Linq;
using CGHSBilling.Areas.HospitalForms.Controllers;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;

namespace CGHSBilling_UnitTestProject.Controllers
{
    [TestFixture]
    public class RequestSubmissionOPDControllerTests
    {
        private Mock<HttpContextBase> _mockHttpContext;
        private Mock<HttpSessionStateBase> _mockSession;
        private RequestSubmissionOPDController _controller;

        [SetUp]
        public void Setup()
        {
            _mockHttpContext = new Mock<HttpContextBase>();
            _mockSession = new Mock<HttpSessionStateBase>();

            _controller = new RequestSubmissionOPDController();
            _mockHttpContext.Setup(ctx => ctx.Session).Returns(_mockSession.Object);
            _controller.ControllerContext = new ControllerContext(_mockHttpContext.Object, new RouteData(), _controller);
        }

        [Test]
        public void CalculateOPDBill_ReturnsJsonSuccess()
        {
            var model = new RequestSubmissionOPDModel
            {
                ConsumeDiv = new List<CommonMasterModel>(),
                ManullyAddedService = new List<CommonMasterModel>(),
                DefaultServices = new List<CommonMasterModel>(),
                HospitalTypeId = 1,
                PatientTypeId = 1,
                StateId = 1,
                CityId = 1,
                GenderId = 1
            };
            _mockSession.Setup(s => s["AppUserId"]).Returns(123);

            var result = _controller.CalculateOPDBill(model) as JsonResult;

            Assert.IsNotNull(result);
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
        }

        [Test]
        public void GetAllOPDRequest_ReturnsJsonWithDataOrError()
        {
            _mockSession.Setup(s => s["AppUserId"]).Returns(123);

            var result = _controller.GetAllOPDRequest() as JsonResult;

            Assert.IsNotNull(result);
        }
    }
}
