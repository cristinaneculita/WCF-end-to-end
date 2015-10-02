using System;
using GeoLib.Contracts;
using GeoLib.Data;
using GeoLib.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GeoLib.Tests
{
    [TestClass]
    public class ManagerTests
    {
        [TestMethod]
        public void test_zip_code_retrieval()
        {

            Mock<IZipCodeRepository> mockZipRepository = new Mock<IZipCodeRepository>();

            ZipCode zipCode = new ZipCode
            {
                City = "Lincoln Park",
                State = new State() {Abbreviation = "NJ"},
                Zip = "07033"
            };
            mockZipRepository.Setup(obj => obj.GetByZip("07033")).Returns(zipCode);

            IGeoService geoService = new GeoManager(mockZipRepository.Object);

            ZipCodeData zipCodeData = geoService.GetZipInfo("07033");

            Assert.IsTrue(zipCodeData.City.ToUpper() == "LINCOLN PARK");
            Assert.IsTrue(zipCodeData.State == "NJ");
        }
    }
}
