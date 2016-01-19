using System.Runtime.Serialization;

namespace GeoLib.Contracts
{
    [DataContract]
    public class ZipCityData
    {
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
    }
}
