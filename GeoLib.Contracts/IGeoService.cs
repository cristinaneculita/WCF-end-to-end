using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Contracts
{
    [ServiceContract(CallbackContract =typeof(IUpdateZipCallback))]
    public interface IGeoService
    {
        [OperationContract]
        [FaultContract(typeof(ApplicationException))]
        [FaultContract(typeof(NotFoundData))]
        ZipCodeData GetZipInfo(string zip);

        [OperationContract]
        IEnumerable<string> GetStates(bool primaryOnly);

        [OperationContract(Name="GetZipsByState")]
        IEnumerable<ZipCodeData> GetZips(string state);

        [OperationContract(Name="GetZipsForRange")]
        IEnumerable<ZipCodeData> GetZips(string zip, int range);

        [OperationContract(IsOneWay =true)]
        void OneWayExample();

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateZipCity(IEnumerable<ZipCityData> zipCityData);


    }
    [ServiceContract]
    public interface IUpdateZipCallback
    {
        [OperationContract(IsOneWay = true)]
        void ZipUpdated(ZipCityData zipCityData);
    }
}
