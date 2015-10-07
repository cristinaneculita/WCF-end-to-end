using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
        [TestMethod]
        public void integration_test_zip_code_retrieval()
        {
            string address = "net.pipe://localhost/GeoService";
            Binding binding = new NetNamedPipeBinding();

            ServiceHost host = new ServiceHost(typeof (GeoManager));

            host.AddServiceEndpoint(typeof (IGeoService), binding, address);

            host.Open();

            ChannelFactory<IGeoService> factory = new ChannelFactory<IGeoService>(binding, new EndpointAddress(address));
            IGeoService proxy = factory.CreateChannel();

            ZipCodeData zipCodeData = proxy.GetZipInfo("07035");

            factory.Close();

            Assert.IsTrue(zipCodeData.City.ToUpper() == "LINCOLN PARK");
            Assert.IsTrue(zipCodeData.State == "NJ");
        }
    }
}
