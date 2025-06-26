// Unit Tests for HomeController
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
using System.IO;
using System.Web.SessionState;

namespace CGHSBilling_UnitTestProject.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<HttpContextBase> _mockHttpContext;
        private Mock<HttpSessionStateBase> _mockSession;
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            // Arrange
            var httpRequest = new HttpRequest("", "http://localhost/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id",
                new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                10, true, HttpCookieMode.AutoDetect,
                SessionStateMode.InProc, false);

            SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);

            HttpContext.Current = httpContext;
            HttpContext.Current.Session["DatabaseSeLection"] = "DefaultConnection";



            _mockHttpContext = new Mock<HttpContextBase>();
            _mockSession = new Mock<HttpSessionStateBase>();

            _controller = new HomeController();
            _mockHttpContext.Setup(ctx => ctx.Session).Returns(_mockSession.Object);
            _controller.ControllerContext = new ControllerContext(_mockHttpContext.Object, new RouteData(), _controller);
        }

        [Test]
        public void Index_WhenSessionIsNull_RedirectsToLogin()
        {
            _mockSession.Setup(s => s["AppUserId"]).Returns(null);

            var result = _controller.Index() as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [Test]
        public void About_ReturnsViewWithMessage()
        {
            var result = _controller.About() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Your app description page.", result.ViewBag.Message);
        }

        [Test]
        public void Contact_ReturnsViewWithMessage()
        {
            var result = _controller.Contact() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Your contact page.", result.ViewBag.Message);
        }

        [Test]
        public void PackageSummaryView_ReturnsPartialView()
        {
            var result = _controller.PackageSummaryView() as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("_PackageSummaryView", result.ViewName);
        }

        [Test]
        public void AccessDenied_ReturnsView()
        {
            var result = _controller.AccessDenied() as ViewResult;

            Assert.IsNotNull(result);
        }
    }

    
}
