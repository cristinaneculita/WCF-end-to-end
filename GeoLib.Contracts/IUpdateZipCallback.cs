using System.ServiceModel;

namespace GeoLib.Contracts
{
    [ServiceContract]
    public interface IUpdateZipCallback
    {
        [OperationContract(IsOneWay = true)]
        void ZipUpdated(ZipCityData zipCityData);
    }
}