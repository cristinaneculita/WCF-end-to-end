using System.ServiceModel;

namespace GeoLib.Client.Contracts
{
    [ServiceContract]
     public  interface IMessageService
    {
        [OperationContract(Name="ShowMessage")]
        void ShowMsg(string message);
    }
}
