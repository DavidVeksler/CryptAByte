using System;
using System.Web;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CryptAByte.WebUI.Tests
{
    public static class RouteTestHelpers
    {
        public static void AssertRoute(RouteCollection routes, string url, object expectations)
        {
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath)
                .Returns(url);

            RouteData routeData = routes.GetRouteData(httpContextMock.Object);
            Assert.IsNotNull(routeData);

            foreach (var kvp in new RouteValueDictionary(expectations))
            {
                Assert.IsTrue(string.Equals(kvp.Value.ToString(),
                                          routeData.Values[kvp.Key].ToString(),
                                          StringComparison.OrdinalIgnoreCase)
                            , string.Format("Expected '{0}', not '{1}' for '{2}'.",
                                            kvp.Value, routeData.Values[kvp.Key], kvp.Key));
            }
        }
    }
}