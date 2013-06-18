using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CryptAByte.Domain.KeyManager;
using CryptAByte.WebUI;
using CryptAByte.WebUI.Controllers;

namespace CryptAByte.WebUI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            var requestRepository = new Mock<IRequestRepository>();
            HomeController controller = new HomeController(requestRepository.Object);

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("Welcome to ASP.NET MVC!", result.ViewBag.Message);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            var requestRepository = new Mock<IRequestRepository>();
            HomeController controller = new HomeController(requestRepository.Object);

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
