using System.Collections.Generic;
using System.ServiceModel;
using GeoLib.Contracts;

namespace GeoLib.Proxies
{
    public class StatefulGeoClient: ClientBase<IStatefulGeoService>,IStatefulGeoService
    {
        public void PushZip(string zip)
        {
            Channel.PushZip(zip);
        }

        public ZipCodeData GetZipInfo()
        {
            return Channel.GetZipInfo();
        }

        public IEnumerable<ZipCodeData> GetZips(int range)
        {
            return Channel.GetZips(range);
        }
    }
}
