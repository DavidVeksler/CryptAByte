using System;
using System.Web;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CryptAByte.WebUI.Tests
{
    [TestClass]
    public class HomePageTests
    {
        [TestMethod]
        public void CanMapNormalControllerActionRoute()
        {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath)
                .Returns("~/Service/Create");

            RouteData routeData = routes.GetRouteData(httpContextMock.Object);
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Service", routeData.Values["Controller"]);
            Assert.AreEqual("Create", routeData.Values["action"]);
        }

        [TestMethod]
        [Ignore]
        public void RouteHasDefaultActionWhenUrlWithoutAction()
        {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            RouteTestHelpers.AssertRoute(routes, "~/7b6962ab6784cc1f", new { controller = "Home", action = "Details" });
        }
    }
}
